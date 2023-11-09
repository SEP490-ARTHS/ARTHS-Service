using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IAccountService
    {
        Task<ListViewModel<AccountViewModel>> GetAccounts(AccountFilterModel filter, PaginationRequestModel pagination);
        Task<List<AccountViewModel>> GetTellers(AccountFilterModel filter);
        Task<List<AccountViewModel>> GetOwners(AccountFilterModel filter);
        Task<List<AccountViewModel>> GetStaffs(AccountFilterModel filter);
        Task<List<AccountViewModel>> GetCustomers(AccountFilterModel filter);
        Task<Guid> CreateAccount(string phoneNumber, string password, string role);
    }
}
