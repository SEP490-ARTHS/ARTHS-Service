using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using Microsoft.AspNetCore.Http;

namespace ARTHS_Service.Interfaces
{
    public interface IMotobikeProductService
    {
        Task<ListViewModel<MotobikeProductDetailViewModel>> GetMotobikeProducts(MotobikeProductFilterModel filter, PaginationRequestModel pagination);
        Task<MotobikeProductDetailViewModel> GetMotobikeProduct(Guid id);
        Task<MotobikeProductDetailViewModel> CreateMotobikeProduct(CreateMotobikeProductModel model);
        Task<MotobikeProductDetailViewModel> UpdateMotobikeProduct(Guid id, UpdateMotobikeProductModel model);
        Task<MotobikeProductDetailViewModel> UpdateMotobikeProductImage(Guid motobikeProductId, UpdateImageModel model);
        Task RemoveMotobikeProductImage(Guid imageId);
    }
}
