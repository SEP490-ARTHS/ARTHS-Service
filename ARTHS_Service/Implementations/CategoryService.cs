using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Service.Implementations
{
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _categoryRepository = unitOfWork.Category;
        }

        public async Task<CategoryViewModel> GetCategory(Guid id)
        {
            return await _categoryRepository.GetMany(category => category.Id.Equals(id))
                            .ProjectTo<CategoryViewModel>(_mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync() ?? null!;

        }

        public async Task<List<CategoryViewModel>> GetCategories(CategoryFilterModel filter)
        {
            var query = _categoryRepository.GetAll();
            if (filter.Name != null)
            {
                query = query.Where(category => category.CategoryName.Contains(filter.Name));
            }

            return await query.
                ProjectTo<CategoryViewModel>(_mapper.ConfigurationProvider)
                .OrderBy(category => category.CategoryName)
                .ToListAsync();
        }

        public async Task<CategoryViewModel> CreateCategory(CreateCategoryRequest request)
        {
            try
            {

                if (_categoryRepository.Any(category => category.CategoryName.Equals(request.CategoryName)))
                {
                    throw new ConflictException("Danh mục này đã tồn tại!");
                }

                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    CategoryName = request.CategoryName,
                };

                _categoryRepository.Add(category);

                var result = await _unitOfWork.SaveChanges().ConfigureAwait(false);

                if (result > 0)
                {
                    return await GetCategory(category.Id);
                }

                throw new Exception("Tạo thất bại!");
            }
            catch (Exception ex)
            {
                // Log the exception here if logging is implemented.
                throw new InvalidOperationException("An error occurred while creating the category.", ex);
            }
        }


        public async Task<CategoryViewModel> UpdateCategory(Guid Id, UpdateCategoryRequest request)
        {
            try
            {
                var category = await _categoryRepository.GetMany(c => c.Id.Equals(Id)).FirstOrDefaultAsync();

                if (category == null)
                {
                    throw new NotFoundException("không tìm thấy");
                }


                if (_categoryRepository.Any(c => c.CategoryName.Equals(request.Name) && c.Id != Id))
                {
                    throw new ConflictException("Tên danh mục đã tồn tại");
                }

                category.CategoryName = request.Name ?? category.CategoryName;

                _categoryRepository.Update(category);

                var result = await _unitOfWork.SaveChanges();

                if (result > 0)
                {
                    return await GetCategory(Id);
                }

                throw new Exception("thay đổi thất bại");
            }
            catch (Exception ex)
            {
                // Log the exception here if logging is implemented.
                throw new Exception("An error occurred while updating the category.", ex);
            }
        }


        public async Task<CategoryViewModel> DeleteCategory(Guid Id)
        {
            var category = await _categoryRepository.GetMany(category => category.Id.Equals(Id)).FirstOrDefaultAsync();
            if (category != null)
            {
                _categoryRepository.Remove(category);

                var result = await _unitOfWork.SaveChanges();
                if (result > 0)
                {
                    return new CategoryViewModel { };
                }
                throw new Exception("xóa không thành công");
            }
            throw new NotFoundException("không tìm thấy");
        }
    }
}
