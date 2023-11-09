using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IFeedbackProductService
    {
        Task<FeedbackProductViewModel> GetFeedback(Guid Id);
        Task<FeedbackProductViewModel> CreateProductFeedback(Guid customerId, CreateFeedbackProductModel model);
        Task<FeedbackProductViewModel> UpdateProductFeedback(Guid customerId, Guid feedbackId, UpdateFeedbackProductModel model);
    }
}
