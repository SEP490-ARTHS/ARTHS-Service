using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service;
using ARTHS_Utility.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Data.Repositories.Implementations
{
    public class WarrantyHistoryService : BaseService, IWarrantyHistoryService
    {
        private readonly IWarrantyHistoryRepository _warrantyHistoryRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        public WarrantyHistoryService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _warrantyHistoryRepository = unitOfWork.WarrantyHistory;
            _orderDetailRepository = unitOfWork.OrderDetail;
        }

        public async Task<ListViewModel<WarrantyHistoryViewModel>> GetWarrantyHistories(PaginationRequestModel pagination)
        {
            var query = _warrantyHistoryRepository.GetAll().AsQueryable();

            var totalRow = await query.AsNoTracking().CountAsync();
            var paginatedQuery = query
                .OrderByDescending(warranty => warranty.RepairDate)
                .Skip(pagination.PageNumber * pagination.PageSize)
                .Take(pagination.PageSize);
            var warranties = await paginatedQuery
                .ProjectTo<WarrantyHistoryViewModel>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();
            return new ListViewModel<WarrantyHistoryViewModel>
            {
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalRow = totalRow
                },
                Data = warranties
            };
        }

        public async Task<WarrantyHistoryViewModel> GetWarrantyHistory(Guid Id)
        {
            return await _warrantyHistoryRepository.GetMany(warranty => warranty.Id.Equals(Id))
                .ProjectTo<WarrantyHistoryViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy thông tin");
        }

        public async Task<WarrantyHistoryViewModel> CreateWarranty(CreateWarrantyModel model)
        {
            var detail = await _orderDetailRepository.GetMany(detail => detail.Id.Equals(model.OrderDetailId)).FirstOrDefaultAsync();
            if (detail == null) throw new NotFoundException($"Không tìm thấy thông tin của detail {model.OrderDetailId}");

            var warranty = new WarrantyHistory
            {
                Id = Guid.NewGuid(),
                OrderDetailId = model.OrderDetailId,
                HandledBy = model.HandledBy,
                RepairDate = DateTime.UtcNow.AddHours(7),
                RepairDetails = model.RepairDetails,
                TotalAmount = model.TotalAmount,
                Status = "Hoàn thành"
            };

            if (detail.MotobikeProductId.HasValue)
            {
                if (!detail.InstUsed)
                {
                    throw new ConflictException("Đơn hàng này bạn không dùng dịch vụ lắp đặt bên chúng tôi.");
                }
                if (detail.Quantity < model.ProductQuantity)
                {
                    throw new ConflictException("Số lượng sản phẩm bảo hành không được lớn hơn số sản phẩm đã mua.");
                }
                warranty.ProductQuantity = (int)model.ProductQuantity!;
            }

            _warrantyHistoryRepository.Add(warranty);
            return await _unitOfWork.SaveChanges() > 0 ? await GetWarrantyHistory(warranty.Id) : null!;
        }

        public async Task<WarrantyHistoryViewModel> UpdateWarranty(Guid Id, UpdateWarrantyModel model)
        {
            var warranty = await _warrantyHistoryRepository.GetMany(warranty => warranty.Id.Equals(Id)).FirstOrDefaultAsync();
            if (warranty == null) throw new NotFoundException("Không tìm thấy thông tin");

            warranty.ProductQuantity = model.ProductQuantity ?? warranty.ProductQuantity;
            warranty.RepairDetails = model.RepairDetails ?? warranty.RepairDetails;
            warranty.Status = model.Status ?? warranty.Status;
            warranty.TotalAmount = model.TotalAmount ?? warranty.TotalAmount;

            _warrantyHistoryRepository.Update(warranty);
            return await _unitOfWork.SaveChanges() > 0 ? await GetWarrantyHistory(Id) : null!;
        }
    }
}
