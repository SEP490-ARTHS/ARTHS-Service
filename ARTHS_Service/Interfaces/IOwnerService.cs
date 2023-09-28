using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IOwnerService
    {
        Task<OwnerViewModel> GetOwner(Guid id);
    }
}
