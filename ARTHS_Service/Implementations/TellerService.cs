using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Exceptions;
using ARTHS_Utility.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Service.Implementations
{
    public class TellerService : BaseService, ITellerService
    {
        private readonly ITellerRepository _tellerRepository;

        private readonly IAccountService _accountService;
        private readonly ICloudStorageService _cloudStorageService;

        public TellerService(IUnitOfWork unitOfWork, IMapper mapper, IAccountService accountService, ICloudStorageService cloudStorageService) : base(unitOfWork, mapper)
        {
            _tellerRepository = unitOfWork.Teller;

            _accountService = accountService;
            _cloudStorageService = cloudStorageService;
        }

        public async Task<TellerViewModel> GetTeller(Guid id)
        {
            return await _tellerRepository.GetMany(teller => teller.AccountId.Equals(id))
                .ProjectTo<TellerViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy teller.");
        }


        public async Task<TellerViewModel> CreateTeller(RegisterTellerModel model)
        {
            var result = 0;
            var accountId = Guid.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    //create owner account
                    accountId = await _accountService.CreateAccount(model.PhoneNumber, model.Password, UserRole.Teller);

                    var teller = new TellerAccount
                    {
                        AccountId = accountId,
                        FullName = model.FullName,
                        Gender = model.Gender,
                    };

                    _tellerRepository.Add(teller);

                    result = await _unitOfWork.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };
            return result > 0 ? await GetTeller(accountId) : null!;
        }

        public async Task<TellerViewModel> UpdateTeller(Guid id, UpdateTellerModel model)
        {
            var teller = await _tellerRepository.GetMany(teller => teller.AccountId.Equals(id))
                                                .Include(teller => teller.Account)
                                                .FirstOrDefaultAsync();
            if (teller != null)
            {
                teller.FullName = model.FullName ?? teller.FullName;
                teller.Gender = model.Gender ?? teller.Gender;

                if (!string.IsNullOrEmpty(model.OldPassword))
                {
                    if (!PasswordHasher.VerifyPassword(model.OldPassword, teller.Account.PasswordHash))
                    {
                        throw new InvalidOldPasswordException("Mật khẩu cũ không chính sát.");
                    }
                    if (model.NewPassword != null)
                    {
                        teller.Account.PasswordHash = PasswordHasher.HashPassword(model.NewPassword);
                    }
                }
                _tellerRepository.Update(teller);
            }
            else
            {
                throw new NotFoundException("Không tìm thấy teller");
            }
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetTeller(teller.AccountId) : null!;
        }

        public async Task<TellerViewModel> UploadAvatar(Guid id, IFormFile image)
        {
            var teller = await _tellerRepository.GetMany(teller => teller.AccountId.Equals(id)).FirstOrDefaultAsync();
            if (teller != null)
            {
                //xóa hình cũ trong firebase
                if (!string.IsNullOrEmpty(teller.Avatar))
                {
                    await _cloudStorageService.Delete(id);
                }

                //upload hình mới
                var url = await _cloudStorageService.Upload(id, image.ContentType, image.OpenReadStream());

                teller.Avatar = url;

                _tellerRepository.Update(teller);
            }
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetTeller(id) : null!;
        }
    }
}
