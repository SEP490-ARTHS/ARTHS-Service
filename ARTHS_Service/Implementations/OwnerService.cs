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
    public class OwnerService : BaseService, IOwnerService
    {
        private readonly IOwnerRepository _ownerRepository;

        private readonly IAccountService _accountService;
        private readonly ICloudStorageService _cloudStorageService;

        public OwnerService(IUnitOfWork unitOfWork, IMapper mapper, IAccountService accountService, ICloudStorageService cloudStorageService) : base(unitOfWork, mapper)
        {
            _ownerRepository = unitOfWork.Owner;
            _accountService = accountService;
            _cloudStorageService = cloudStorageService;
        }

        public async Task<OwnerViewModel> GetOwner(Guid id)
        {
            return await _ownerRepository.GetMany(owner => owner.AccountId.Equals(id))
                .ProjectTo<OwnerViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy owner.");
        }

        public async Task<OwnerViewModel> CreateOwner(RegisterOwnerModel model)
        {
            var result = 0;
            var accountId = Guid.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    //create owner account
                    accountId = await _accountService.CreateAccount(model.PhoneNumber, model.Password, UserRole.Owner);

                    var owner = new OwnerAccount
                    {
                        AccountId = accountId,
                        FullName = model.FullName,
                        Gender = model.Gender,
                    };

                    _ownerRepository.Add(owner);

                    result = await _unitOfWork.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };
            return result > 0 ? await GetOwner(accountId) : null!;
        }

        public async Task<OwnerViewModel> UpdateOwner(Guid id, UpdateOwnerModel model)
        {
            var owner = await _ownerRepository.GetMany(owner => owner.AccountId.Equals(id))
                                                .Include(owner => owner.Account)
                                                .FirstOrDefaultAsync();
            if (owner != null)
            {
                owner.FullName = model.FullName ?? owner.FullName;
                owner.Gender = model.Gender ?? owner.Gender;

                if (!string.IsNullOrEmpty(model.OldPassword))
                {
                    if (!PasswordHasher.VerifyPassword(model.OldPassword, owner.Account.PasswordHash))
                    {
                        throw new InvalidOldPasswordException("Mật khẩu cũ không chính sát.");
                    }
                    if (model.NewPassword != null)
                    {
                        owner.Account.PasswordHash = PasswordHasher.HashPassword(model.NewPassword);
                    }
                }
                _ownerRepository.Update(owner);
            }
            else
            {
                throw new NotFoundException("Không tìm thấy owner");
            }
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetOwner(owner.AccountId) : null!;
        }

        public async Task<OwnerViewModel> UploadAvatar(Guid id, IFormFile image)
        {
            var owner = await _ownerRepository.GetMany(owner => owner.AccountId.Equals(id)).FirstOrDefaultAsync();
            if (owner != null)
            {
                //xóa hình cũ trong firebase
                if (!string.IsNullOrEmpty(owner.Avatar))
                {
                    await _cloudStorageService.Delete(id);
                }

                //upload hình mới
                var url = await _cloudStorageService.Upload(id, image.ContentType, image.OpenReadStream());

                owner.Avatar = url;

                _ownerRepository.Update(owner);
            }
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetOwner(id) : null!;
        }


    }
}
