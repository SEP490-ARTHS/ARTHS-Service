using ARTHS_Data;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Implementations;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARTHS_Service.Implementations
{
    public class OwnerService : BaseService, IOwnerService
    {
        private readonly IOwnerRepository _ownerRepository;
        public OwnerService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _ownerRepository = unitOfWork.Owner;
        }

        public async Task<OwnerViewModel> GetOwner(Guid id)
        {
            return await _ownerRepository.GetMany(owner => owner.AccountId.Equals(id))
                .Include(owner => owner.Account)
                .ProjectTo<OwnerViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? null!;
        }

    }
}
