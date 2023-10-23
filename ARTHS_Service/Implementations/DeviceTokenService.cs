using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARTHS_Service.Implementations
{
    public class DeviceTokenService : BaseService, IDeviceTokenService
    {
        private readonly IDeviceTokenRepository _deviceToken;
        public DeviceTokenService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _deviceToken = unitOfWork.DeviceToken;
        }

        public async Task<bool> CreateDeviceToken(Guid accountId, CreateDeviceTokenModel model)
        {
            var deviceTokens = await _deviceToken.GetMany(token => token.AccountId.Equals(accountId)).ToListAsync();
            if (deviceTokens.Any(token => token.Token!.Equals(model.DeviceToken))) return false;

            var newDeviceToken = new DeviceToken
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                Token = model.DeviceToken
            };

            _deviceToken.Add(newDeviceToken);
            var result = await _unitOfWork.SaveChanges();
            return result > 0;
        }
    }
}
