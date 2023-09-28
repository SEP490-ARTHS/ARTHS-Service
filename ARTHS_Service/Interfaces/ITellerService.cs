using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface ITellerService
    {
        Task<TellerViewModel> GetTeller(Guid id);
    }
}
