using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IMotobikeProductService
    {
        Task<List<MotobikeProductViewModel>> GetMotobikeProducts(MotobikeProductFilterModel filter);
        Task<MotobikeProductDetailViewModel> GetMotobikeProduct(Guid id);
        Task<MotobikeProductDetailViewModel> CreateMotobikeProduct(CreateMotobikeProductModel model);
        Task<MotobikeProductDetailViewModel> UpdateMotobikeProduct(Guid id, UpdateMotobikeProductModel model);
    }
}
