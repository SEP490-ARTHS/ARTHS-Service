using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using Microsoft.AspNetCore.Http;

namespace ARTHS_Service.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerViewModel> GetCustomer(Guid id);
        Task<CustomerViewModel> CreateCustomer(RegisterCustomerModel model);
        Task<CustomerViewModel> UpdateCustomer(Guid id, UpdateCustomerModel model);
        Task<CustomerViewModel> UploadAvatar(Guid id, IFormFile image);
    }
}
