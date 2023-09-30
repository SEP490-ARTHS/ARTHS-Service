using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryViewModel> GetCategory(Guid id);
        Task<List<CategoryViewModel>> GetCategories(CategoryFilterModel filter);
        Task<CategoryViewModel> CreateCategory(CreateCategoryRequest request);
        Task<CategoryViewModel> UpdateCategory(Guid Id, UpdateCategoryRequest request);
        Task<CategoryViewModel> DeleteCategory(Guid Id);
    }
}
