using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
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

            if(filter.Id != null)
            {
                query = query.Where(order => order.Id.Contains(filter.Id));
            }
            if(filter.CustomerName != null)
            {
                query = query.Where(order => (order.CustomerName != null && order.CustomerName.Contains(filter.CustomerName)));
            }
            if (filter.CustomerPhone != null)
            {
                query = query.Where(order => order.CustomerPhone.Contains(filter.CustomerPhone));
            }

            // Phân trang
            if (filter.PageSize <= 0) filter.PageSize = 10;  // kích thước trang luôn dương
            int skip = (filter.PageNumber - 1) * filter.PageSize; // Tính số items cần bỏ qua
            query = _inStoreOrderRepository.SkipAndTake(skip, filter.PageSize);
            
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

                    var inStoreOrder = new InStoreOrder
                    {
                        Id = inStoreOrderId,
                        TellerId = tellerId,
                        StaffId = model.StaffId,
                        CustomerName = model.CustomerName,
                        CustomerPhone = model.CustomerPhone,
                        LicensePlate = model.LicensePlate,
                        Status = InStoreOrderStatus.NewOrder,
                        OrderType = model.OrderType,
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

                int productTotalPrice = 0;
                if (productPrice.HasValue && productPrice.Value > 0 && detail.ProductQuantity.HasValue && detail.ProductQuantity.Value > 0)
                {
                    productTotalPrice = productPrice.Value * detail.ProductQuantity.Value;
                }

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
                    ProductQuantity = detail.ProductQuantity,
                    ProductPrice = productPrice,
                    ServicePrice = servicePrice,
                    WarrantyPeriod = warrantyPeriod
                };

                listOrderDetail.Add(orderDetail);
            }
            _inStoreOrderDetailRepository.AddRange(listOrderDetail);
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
