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
    public class StaffService : BaseService, IStaffService
    {
        private readonly IStaffRepository _staffRepository;

        private readonly IAccountService _accountService;
        private readonly ICloudStorageService _cloudStorageService;
        public StaffService(IUnitOfWork unitOfWork, IMapper mapper, IAccountService accountService, ICloudStorageService cloudStorageService) : base(unitOfWork, mapper)
        {
            _staffRepository = unitOfWork.Staff;
            _accountService = accountService;
            _cloudStorageService = cloudStorageService;
        }

        public async Task<StaffDetailViewModel> GetStaff(Guid id)
        {
            return await _staffRepository.GetMany(staff => staff.AccountId.Equals(id))
                .ProjectTo<StaffDetailViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy staff.");
        }

        public async Task<StaffDetailViewModel> CreateStaff(RegisterStaffModel model)
        {
            var result = 0;
            var accountId = Guid.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    //create owner account
                    accountId = await _accountService.CreateAccount(model.PhoneNumber, model.Password, UserRole.Staff);

                    var staff = new StaffAccount
                    {
                        AccountId = accountId,
                        FullName = model.FullName,
                        Gender = model.Gender,
                    };

                    _staffRepository.Add(staff);

                    result = await _unitOfWork.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };
            return result > 0 ? await GetStaff(accountId) : null!;
        }

        public async Task<StaffDetailViewModel> UpdateStaff(Guid id, UpdateStaffModel model)
        {
            var staff = await _staffRepository.GetMany(staff => staff.AccountId.Equals(id))
                                                .Include(staff => staff.Account)
                                                .FirstOrDefaultAsync();
            if (staff != null)
            {
                staff.FullName = model.FullName ?? staff.FullName;
                staff.Gender = model.Gender ?? staff.Gender;

                if (!string.IsNullOrEmpty(model.OldPassword))
                {
                    if (!PasswordHasher.VerifyPassword(model.OldPassword, staff.Account.PasswordHash))
                    {
                        throw new InvalidOldPasswordException("Mật khẩu cũ không chính sát.");
                    }
                    if (model.NewPassword != null)
                    {
                        staff.Account.PasswordHash = PasswordHasher.HashPassword(model.NewPassword);
                    }
                }
                _staffRepository.Update(staff);
            }
            else
            {
                throw new NotFoundException("Không tìm thấy staff");
            }
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetStaff(staff.AccountId) : null!;
        }

        public async Task<StaffDetailViewModel> UploadAvatar(Guid id, IFormFile image)
        {
            var staff = await _staffRepository.GetMany(staff => staff.AccountId.Equals(id)).FirstOrDefaultAsync();
            if (staff != null)
            {
                //xóa hình cũ trong firebase
                if (!string.IsNullOrEmpty(staff.Avatar))
                {
                    await _cloudStorageService.Delete(id);
                }

                //upload hình mới
                var url = await _cloudStorageService.Upload(id, image.ContentType, image.OpenReadStream());

                staff.Avatar = url;

                _staffRepository.Update(staff);
            }
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetStaff(id) : null!;
        }
    }
}
