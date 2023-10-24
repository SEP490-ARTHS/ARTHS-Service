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
        private readonly IMotobikeProductRepository _motobikeProductRepository;

        public DiscountService(IUnitOfWork unitOfWork, IMapper mapper, ICloudStorageService cloudStorageService) : base(unitOfWork, mapper)
        {
            _repository = unitOfWork.Discount;
            _motobikeProductRepository = _unitOfWork.MotobikeProduct;
            _cloudStorageService = cloudStorageService;
        }

        public async Task<List<DiscountViewModel>> GetDiscounts(DiscountFilterModel filter)
        {
            var currentTime = DateTime.Now;

            var query = _repository.GetAll();
            if (query != null)
            {
                var discountsToDiscontinue = query
                        .Where(discount => discount.EndDate < currentTime && discount.Status != DiscountStatus.Discontinued)
                        .ToList();

                foreach (var discount in discountsToDiscontinue)
                {
                    discount.Status = DiscountStatus.Discontinued;
                    _repository.Update(discount);
                }

                await _unitOfWork.SaveChanges();
            }
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
            else if (filter.status != null)
            {
                query = query.Where(discount => discount.Status == filter.status);
            }
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
                    if (model.EndDate < DateTime.Now)
                    {
                        throw new BadRequestException("thời gian khuyến mãi phải lớn hơn hiện tại");
                    }
                    else if (model.StartDate > model.EndDate)
                    {
                        throw new BadRequestException("thời gian khuyến mãi sai");
                    }
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
                    await AddDiscountIdIntoMotobikeProduct(discountId, model.MotobikeProductId);
                    result = await _unitOfWork.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
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
            if (discount.EndDate > DateTime.Now)
            {
                discount.Status = DiscountStatus.Active;
                await AddDiscountIdIntoMotobikeProduct(id, model.MotobikeProductId);
            }
            else
            {
                throw new BadRequestException("thời gian khuyến mãi phải lớn hơn hiện tại");
            }
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

        public async Task<DiscountViewModel> DiscontinuedDiscount(Guid id)
        {
            var discount = await _repository.GetMany(d => d.Id.Equals(id)).FirstOrDefaultAsync();
            if (discount == null)
            {
                throw new NotFoundException("Không tìm thấy khuyến mãi");
            }
            discount.Status = DiscountStatus.Discontinued;
            _repository.Update(discount);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetDiscount(id) : null!;
        }
        public async Task<DiscountViewModel> RemoveDiscountInProduct(Guid id)
        {
            var product = await _motobikeProductRepository.GetMany(p => p.Id == id).FirstOrDefaultAsync();
            if (product != null)
            {
                product.DiscountId = null;
                var result = await _unitOfWork.SaveChanges();
                if (result > 0)
                {
                    return new DiscountViewModel { };
                }
                throw new Exception("xóa không thành công");
            }
            throw new NotFoundException("không tìm thấy");
        }

        private async Task<ICollection<MotobikeProduct>> AddDiscountIdIntoMotobikeProduct(Guid idDiscount, ICollection<Guid> idProducts)
        {
            var listProduct = new List<MotobikeProduct>();
            foreach (Guid product in idProducts)
            {
                // Find the motobike product by its ID.
                var motobikeProduct = await _motobikeProductRepository
                    .GetMany(p => p.Id == product)
                    .FirstOrDefaultAsync();

                if (motobikeProduct != null)
                {
                    // Update the DiscountId for the motobike product.
                    motobikeProduct.DiscountId = idDiscount;
                    _motobikeProductRepository.Update(motobikeProduct);
                    listProduct.Add(motobikeProduct);
                }
            }
            return listProduct;
        }


    }
}
