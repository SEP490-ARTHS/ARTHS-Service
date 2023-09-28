using ARTHS_Data;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Service.Implementations
{
    public class TellerService : BaseService, ITellerService
    {
        private readonly ITellerRepository _tellerRepository;
        public TellerService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _tellerRepository = unitOfWork.Teller;
        }

        public async Task<TellerViewModel> GetTeller(Guid id)
        {
            return await _tellerRepository.GetMany(teller => teller.AccountId.Equals(id))
                .Include(teller => teller.Account)
                .ProjectTo<TellerViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? null!;
        }
    }
}
