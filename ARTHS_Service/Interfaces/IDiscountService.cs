using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IDiscountService
    {
        public Task<ListViewModel<BasicDiscountViewModel>> GetDiscounts(DiscountFilterModel filter, PaginationRequestModel pagination);
        public Task<DiscountViewModel> GetDiscount(Guid id);
        public Task<DiscountViewModel> CreateDiscount(CreateDiscountModel model);
        public Task<DiscountViewModel> UpdateDiscount(Guid id, UpdateDiscountModel model);
        public Task<DiscountViewModel> RemoveDiscountInProduct(Guid id);
        public Task<DiscountViewModel> RemoveDiscountInService(Guid id);
        public Task<DiscountViewModel> DiscontinuedDiscount(Guid id);
        public Task CheckDicounts();
    }
}