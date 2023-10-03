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
    public class DiscountService : BaseService, IDiscountService
    {
        private readonly IDiscountRepository _repository;
        private readonly ICloudStorageService _cloudStorageService;

        public DiscountService(IUnitOfWork unitOfWork, IMapper mapper, ICloudStorageService cloudStorageService) : base(unitOfWork, mapper)
        {
            _repository = unitOfWork.Discount;
            _cloudStorageService = cloudStorageService;
        }

        public async Task<List<DiscountViewModel>> GetDiscounts(DiscountFilterModel filter)
        {
            var query = _repository.GetAll();

            if (filter.Title != null)
            {
                query = query.Where(discount => discount.Title.Contains(filter.Title));
            }
            if (filter.StartDate != null && filter.EndDate != null)
            {
                query = query.Where(discount => discount.StartDate >= filter.StartDate && discount.EndDate <= filter.EndDate);
            }
            else if (filter.StartDate != null)
            {
                query = query.Where(discount => discount.StartDate >= filter.StartDate);
            }
            else if (filter.EndDate != null)
            {
                query = query.Where(discount => discount.EndDate <= filter.EndDate);
            }

            // Phân trang
            if (filter.PageSize <= 0) filter.PageSize = 10;  // kích thước trang luôn dương
            int skip = (filter.PageNumber - 1) * filter.PageSize; // Tính số items cần bỏ qua
            query = _repository.SkipAndTake(skip, filter.PageSize);

            return await query
                .ProjectTo<DiscountViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<DiscountViewModel> GetDiscount(Guid id)
        {
            return await _repository.GetMany(discount => discount.Id.Equals(id))
                .ProjectTo<DiscountViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy khuyến mãi");
        }

        public async Task<DiscountViewModel> CreateDiscount(CreateDiscountModel model)
        {
            var result = 0;
            var discountId = Guid.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    discountId = Guid.NewGuid();
                    var imageUrl = await _cloudStorageService.Upload(discountId, model.Image.ContentType, model.Image.OpenReadStream());
                    var discount = new Discount
                    {
                        Id = discountId,
                        Title = model.Title,
                        DiscountAmount = model.DiscountAmount,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        ImageUrl = imageUrl,
                        Description = model.Description,
                        Status = DiscountStatus.Active
                    };

                    _repository.Add(discount);
                    result = await _unitOfWork.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            };
            return result > 0 ? await GetDiscount(discountId) : null!;

        }

        public async Task<DiscountViewModel> UpdateDiscount(Guid id, UpdateDiscountModel model)
        {
            var discount = await _repository.GetMany(d => d.Id.Equals(id)).FirstOrDefaultAsync();

            if (discount == null)
            {
                throw new NotFoundException("Không tìm thấy khuyến mãi");
            }

            discount.Title = model.Title ?? discount.Title;
            discount.DiscountAmount = model.DiscountAmount ?? discount.DiscountAmount;
            discount.StartDate = model.StartDate ?? discount.StartDate;
            discount.EndDate = model.EndDate ?? discount.EndDate;
            discount.Description = model.Description ?? discount.Description;
            discount.Status = model.Status ?? discount.Status;

            if (model.Image != null)
            {
                await _cloudStorageService.Delete(id);
                var newImageUrl = await _cloudStorageService.Upload(id, model.Image.ContentType, model.Image.OpenReadStream());
                discount.ImageUrl = newImageUrl;
            }

            _repository.Update(discount);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetDiscount(id) : null!;
        }

    }
}
