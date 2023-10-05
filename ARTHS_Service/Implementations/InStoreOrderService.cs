using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Service.Implementations
{
    public class InStoreOrderService : BaseService, IInStoreOrderService
    {
        private readonly IInStoreOrderRepository _inStoreOrderRepository;
        private readonly IInStoreOrderDetailRepository _inStoreOrderDetailRepository;
        private readonly IMotobikeProductRepository _motobikeProductRepository;
        private readonly IRepairServiceRepository _repairServiceRepository;

        public InStoreOrderService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _inStoreOrderRepository = unitOfWork.InStoreOrder;
            _inStoreOrderDetailRepository = unitOfWork.InStoreOrderDetail;
            _motobikeProductRepository = unitOfWork.MotobikeProduct;
            _repairServiceRepository = unitOfWork.RepairService;
        }
        public async Task<List<BasicInStoreOrderViewModel>> GetInStoreOrders(InStoreOrderFilterModel filter)
        {
            var query = _inStoreOrderRepository.GetAll();

            if (filter.Id != null)
            {
                query = query.Where(order => order.Id.Contains(filter.Id));
            }
            if (filter.CustomerName != null)
            {
                query = query.Where(order => (order.CustomerName != null && order.CustomerName.Contains(filter.CustomerName)));
            }
            if (filter.CustomerPhone != null)
            {
                query = query.Where(order => order.CustomerPhone.Contains(filter.CustomerPhone));
            }

            return await query
                .ProjectTo<BasicInStoreOrderViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();

        }

        public async Task<InStoreOrderViewModel> GetInStoreOrder(string id)
        {
            return await _inStoreOrderRepository.GetMany(order => order.Id.Equals(id))
                .ProjectTo<InStoreOrderViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy order");
        }

        public async Task<InStoreOrderViewModel> CreateInStoreOrder(Guid tellerId, CreateInStoreOrderModel model)
        {
            var result = 0;
            var inStoreOrderId = string.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    inStoreOrderId = GenerateOrderId();
                    var totalPrice = await CreateInStoreOrderDetail(inStoreOrderId, model.OrderDetailModel);
                    bool isPurchaseOrder = model.OrderDetailModel.All(detail => !detail.RepairServiceId.HasValue); //nếu không có thì là đơn mua
                    var inStoreOrder = new InStoreOrder
                    {
                        Id = inStoreOrderId,
                        TellerId = tellerId,
                        StaffId = model.StaffId,
                        CustomerName = model.CustomerName,
                        CustomerPhone = model.CustomerPhone,
                        LicensePlate = model.LicensePlate,
                        Status = InStoreOrderStatus.NewOrder,
                        OrderType = isPurchaseOrder ? "Purchase" : "Repair",
                        TotalAmount = totalPrice
                    };
                    _inStoreOrderRepository.Add(inStoreOrder);

                    result = await _unitOfWork.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };
            return result > 0 ? await GetInStoreOrder(inStoreOrderId) : null!;
        }

        public async Task<InStoreOrderViewModel> UpdateInStoreOrder(string orderId, UpdateInStoreOrderModel model)
        {
            var inStoreOrder = await _inStoreOrderRepository.GetMany(order => order.Id.Equals(orderId)).FirstOrDefaultAsync();
            if (inStoreOrder == null)
            {
                throw new NotFoundException("Không tìm thấy order.");
            }
            if (inStoreOrder.Status.Equals(InStoreOrderStatus.Finished))
            {
                throw new BadRequestException("Đơn đã hoàn thành không thể chỉnh sữa");
            }

            bool isPurchaseOrder = model.OrderDetailModel != null && model.OrderDetailModel.All(detail => !detail.RepairServiceId.HasValue); //nếu không có thì là đơn mua
            
            inStoreOrder.OrderType = isPurchaseOrder ? "Purchase" : "Repair";
            inStoreOrder.StaffId = model.StaffId ?? inStoreOrder.StaffId;
            inStoreOrder.CustomerName = model.CustomerName ?? inStoreOrder.CustomerName;
            inStoreOrder.CustomerPhone = model.CustomerPhone ?? inStoreOrder.CustomerPhone;
            inStoreOrder.LicensePlate = model.LicensePlate ?? inStoreOrder.LicensePlate;
            inStoreOrder.Status = model.Status ?? inStoreOrder.Status;

            if (model.OrderDetailModel != null && model.OrderDetailModel.Count > 0)
            {
                inStoreOrder.TotalAmount = await UpdateInStoreOrder(orderId, model.OrderDetailModel);
            }

            _inStoreOrderRepository.Update(inStoreOrder);
            return await _unitOfWork.SaveChanges() > 0 ? await GetInStoreOrder(orderId) : null!;
        }


        //PRIVATE METHOD
        private async Task<int> CreateInStoreOrderDetail(string orderId, List<CreateInStoreOrderDetailModel> listDetails)
        {
            int totalAmount = 0;
            var listOrderDetail = new List<InStoreOrderDetail>();
            foreach (var detail in listDetails)
            {
                var product = await _motobikeProductRepository.GetMany(product => product.Id.Equals(detail.MotobikeProductId)).Include(product => product.Warranty).FirstOrDefaultAsync();
                var repairService = await _repairServiceRepository.GetMany(repair => repair.Id.Equals(detail.RepairServiceId)).FirstOrDefaultAsync();

                int? productPrice = product?.PriceCurrent;
                int? warrantyOfProduct = product?.Warranty?.Duration;
                int? servicePrice = repairService?.Price;

                //int productTotalPrice = 0;
                //if (productPrice.HasValue && productPrice.Value > 0 && detail.ProductQuantity.HasValue && detail.ProductQuantity.Value > 0)
                //{
                //    productTotalPrice = productPrice.Value * detail.ProductQuantity.Value;
                //}
                int productTotalPrice = productPrice.HasValue && productPrice.Value > 0
                    ? productPrice.Value * (detail.ProductQuantity == null || detail.ProductQuantity == 0 ? 1 : detail.ProductQuantity.Value) : 0;
                int serviceTotalPrice = servicePrice.HasValue && servicePrice.Value > 0 ? servicePrice.Value : 0;

                int detailTotalPrice = productTotalPrice + serviceTotalPrice;

                totalAmount += detailTotalPrice; // Cộng dồn vào tổng tiền

                DateTime warrantyPeriod = warrantyOfProduct.HasValue ? DateTime.UtcNow.AddMonths(warrantyOfProduct.Value) : DateTime.UtcNow;

                var orderDetail = new InStoreOrderDetail
                {
                    Id = Guid.NewGuid(),
                    InStoreOrderId = orderId,
                    RepairServiceId = detail.RepairServiceId,
                    MotobikeProductId = detail.MotobikeProductId,
                    ProductQuantity = detail.ProductQuantity == null || detail.ProductQuantity == 0 ? 1 : detail.ProductQuantity,
                    ProductPrice = productPrice,
                    ServicePrice = servicePrice,
                    WarrantyPeriod = warrantyPeriod
                };

                listOrderDetail.Add(orderDetail);
            }
            _inStoreOrderDetailRepository.AddRange(listOrderDetail);
            return totalAmount;
        }

        private async Task<int> UpdateInStoreOrder(string orderId, List<CreateInStoreOrderDetailModel> listDetails)
        {
            int totalAmount = 0;
            var listOrderDetail = new List<InStoreOrderDetail>();

            //xóa details cũ
            var existDetails = await _inStoreOrderDetailRepository.GetMany(detail => detail.InStoreOrderId.Equals(orderId)).ToListAsync();
            _inStoreOrderDetailRepository.RemoveRange(existDetails);

            // Thêm các chi tiết đơn hàng mới từ model
            foreach (var detail in listDetails)
            {
                var product = await _motobikeProductRepository.GetMany(product => product.Id.Equals(detail.MotobikeProductId)).Include(product => product.Warranty).FirstOrDefaultAsync();
                var repairService = await _repairServiceRepository.GetMany(repair => repair.Id.Equals(detail.RepairServiceId)).FirstOrDefaultAsync();

                int? productPrice = product?.PriceCurrent;
                int? warrantyOfProduct = product?.Warranty?.Duration;
                int? servicePrice = repairService?.Price;

                int productTotalPrice = productPrice.HasValue && productPrice.Value > 0
                    ? productPrice.Value * (detail.ProductQuantity == null || detail.ProductQuantity == 0 ? 1 : detail.ProductQuantity.Value) : 0;    
                int serviceTotalPrice = servicePrice.HasValue && servicePrice.Value > 0 ? servicePrice.Value : 0;

                int detailTotalPrice = productTotalPrice + serviceTotalPrice;

                totalAmount += detailTotalPrice; // Cộng dồn vào tổng tiền

                DateTime warrantyPeriod = warrantyOfProduct.HasValue ? DateTime.UtcNow.AddMonths(warrantyOfProduct.Value) : DateTime.UtcNow;

                var orderDetail = new InStoreOrderDetail
                {
                    Id = Guid.NewGuid(),
                    InStoreOrderId = orderId,
                    RepairServiceId = detail.RepairServiceId,
                    MotobikeProductId = detail.MotobikeProductId,
                    ProductQuantity = detail.ProductQuantity == null || detail.ProductQuantity == 0 ? 1 : detail.ProductQuantity,
                    ProductPrice = productPrice,
                    ServicePrice = servicePrice,
                    WarrantyPeriod = warrantyPeriod
                };

                listOrderDetail.Add(orderDetail);
            }

            _inStoreOrderDetailRepository.AddRange(listOrderDetail);
            await _unitOfWork.SaveChanges();
            return totalAmount;

        }

        private string GenerateOrderId()
        {
            DateTime now = DateTime.Now;
            // Lấy tổng số giây từ thời điểm đầu tiên của năm hiện tại tới thời điểm hiện tại
            int seconds = (int)(now - new DateTime(now.Year, 1, 1)).TotalSeconds;
            // Lấy phần dư khi chia cho 10^8 để giữ ID dưới 8 ký tự
            int uniqueNumber = seconds % 100000000;
            // Tạo ID dưới dạng "OR" + 8 ký tự số
            string id = "OR" + uniqueNumber.ToString("D8"); // D8 giúp đảm bảo có đủ 8 chữ số
            return id;
        }
    }
}
