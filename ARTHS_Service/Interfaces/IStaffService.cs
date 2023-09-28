using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IStaffService
    {
        Task<StaffViewModel> GetStaff(Guid id);
    }
}
