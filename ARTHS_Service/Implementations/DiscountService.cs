using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Get;
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
        private readonly IDiscountRepository _discountRepository;
        private readonly ICloudStorageService _cloudStorageService;
        private readonly IMotobikeProductRepository _motobikeProductRepository;
        private readonly IRepairServiceRepository _repairServiceRepository;
        private readonly IMaintenanceScheduleRepository _maintenanceScheduleRepository;

        public DiscountService(IUnitOfWork unitOfWork, IMapper mapper, ICloudStorageService cloudStorageService) : base(unitOfWork, mapper)
        {
            _discountRepository = unitOfWork.Discount;
            _motobikeProductRepository = unitOfWork.MotobikeProduct;
            _cloudStorageService = cloudStorageService;
            _repairServiceRepository = unitOfWork.RepairService;
            _maintenanceScheduleRepository = unitOfWork.MaintenanceSchedule;
        }

        public async Task<ListViewModel<BasicDiscountViewModel>> GetDiscounts(DiscountFilterModel filter, PaginationRequestModel pagination)
        {
            var query = _discountRepository.GetAll().AsQueryable();

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
            var totalRow = await query.AsNoTracking().CountAsync();
            var paginatedQuery = query
                .Skip(pagination.PageNumber * pagination.PageSize)
                .Take(pagination.PageSize);

            var discount = await paginatedQuery
                .ProjectTo<BasicDiscountViewModel>(_mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            return new ListViewModel<BasicDiscountViewModel>
            {
                Pagination = new PaginationViewModel
                {
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize,
                    TotalRow = totalRow
                },
                Data = discount
            };
        }

        public async Task<DiscountViewModel> GetDiscount(Guid id)
        {
            return await _discountRepository.GetMany(discount => discount.Id.Equals(id))
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

                    _discountRepository.Add(discount);
                    if (model.MotobikeProductId != null)
                        await AddDiscountIdIntoMotobikeProduct(discountId, model.MotobikeProductId);

                    if (model.RepairServiceId != null)
                        await AddDiscountIdIntoRepairService(discountId, model.RepairServiceId);
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
            var discount = await _discountRepository.GetMany(d => d.Id.Equals(id)).FirstOrDefaultAsync();

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
                await AddDiscountIdIntoRepairService(id, model.RepairServiceId);
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

            _discountRepository.Update(discount);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetDiscount(id) : null!;
        }

        public async Task<DiscountViewModel> DiscontinuedDiscount(Guid id)
        {
            var discount = await _discountRepository.GetMany(d => d.Id.Equals(id)).FirstOrDefaultAsync();
            if (discount == null)
            {
                throw new NotFoundException("Không tìm thấy khuyến mãi");
            }
            discount.Status = DiscountStatus.Discontinued;
            _discountRepository.Update(discount);
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

        public async Task<DiscountViewModel> RemoveDiscountInService(Guid id)
        {
            var service = await _repairServiceRepository.GetMany(s => s.Id == id).FirstOrDefaultAsync();
            if (service != null)
            {
                service.DiscountId = null;
                var result = await _unitOfWork.SaveChanges();
                if (result > 0)
                {
                    return new DiscountViewModel { };
                }
                throw new Exception("xóa không thành công");
            }
            throw new NotFoundException("không tìm thấy");
        }

        private async Task<ICollection<MotobikeProduct>> AddDiscountIdIntoMotobikeProduct(Guid idDiscount, ICollection<Guid>? idProducts)
        {
            var listProduct = new List<MotobikeProduct>();
            if (idProducts != null)
            {
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
                    else
                    {
                        throw new NotFoundException("không tìm thấy product này: " + product);
                    }
                }
            }
            return listProduct;
        }

        private async Task<ICollection<RepairService>> AddDiscountIdIntoRepairService(Guid idDiscount, ICollection<Guid>? idService)
        {
            var listService = new List<RepairService>();
            if (idService != null)
            {
                foreach (Guid service in idService)
                {
                    var repairService = await _repairServiceRepository
                        .GetMany(s => s.Id == service)
                        .FirstOrDefaultAsync();

                    if (repairService != null)
                    {
                        repairService.DiscountId = idDiscount;
                        _repairServiceRepository.Update(repairService);
                        listService.Add(repairService);
                    }
                    else
                    {
                        throw new NotFoundException("không tìm thấy dịch vụ này: " + service);
                    }
                }
            }
            return listService;
        }

        public async Task CheckDicounts()
        {
            var currentTime = DateTime.UtcNow;

            var discountsToDiscontinue = await _discountRepository
                    .GetMany(discount => discount.EndDate.Date < currentTime.Date && discount.Status != DiscountStatus.Discontinued)
                    .ToListAsync();

            if (discountsToDiscontinue.Count == 0) return;

            foreach (var discount in discountsToDiscontinue)
            {
                discount.Status = DiscountStatus.Discontinued;
            }

            _discountRepository.UpdateRange(discountsToDiscontinue);
            await _unitOfWork.SaveChanges();
        }
    }
}