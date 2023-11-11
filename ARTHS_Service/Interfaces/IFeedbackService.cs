using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IFeedbackService
    {
        Task<FeedbackProductViewModel> GetFeedbackProduct(Guid Id);
        Task<FeedbackProductViewModel> CreateProductFeedback(Guid customerId, CreateFeedbackProductModel model);
        Task<FeedbackProductViewModel> UpdateProductFeedback(Guid customerId, Guid feedbackId, UpdateFeedbackProductModel model);
        Task<FeedbackStaffViewModel> GetFeedbackStaff(Guid Id);
        Task<FeedbackStaffViewModel> CreateFeedbackStaff(CreateFeedbackStaffModel model);

    }
}
