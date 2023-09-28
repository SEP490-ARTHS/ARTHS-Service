using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Enums;
using ARTHS_Utility.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Service.Implementations
{
    public class CustomerService : BaseService, ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountRoleRepository _accountRoleRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICloudStorageService _cloudStorageService;
        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, ICloudStorageService cloudStorageService) : base(unitOfWork, mapper)
        {
            _customerRepository = unitOfWork.Customer;
            _accountRepository = unitOfWork.Account;
            _accountRoleRepository = unitOfWork.AccountRole;
            _cartRepository = unitOfWork.Cart;
            _cloudStorageService = cloudStorageService;
        }


        public async Task<CustomerViewModel> GetCustomer(Guid id)
        {
            return await _customerRepository.GetMany(customer => customer.AccountId.Equals(id))
                .Include(customer => customer.Account)
                .ProjectTo<CustomerViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? null!;
        }


        public async Task<CustomerViewModel> CreateCustomer(RegisterCustomerModel model)
        {
            var result = 0;
            var accountId = Guid.Empty;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    //create account
                    accountId = await CreateAccount(model.PhoneNumber, model.Password);

                    //create customer
                    var customer = new CustomerAccount
                    {
                        AccountId = accountId,
                        FullName = model.FullName,
                        Gender = model.Gender,
                        Address = model.Address,
                    };
                    _customerRepository.Add(customer);

                    //create cart
                    var cart = new Cart
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = accountId,
                    };
                    _cartRepository.Add(cart);

                    result = await _unitOfWork.SaveChanges();
                    transaction.Commit();

                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };
            return result > 0 ? await GetCustomer(accountId) : null!;
        }

        public async Task<CustomerViewModel> UpdateCustomer(Guid id, UpdateCustomerModel model)
        {
            var customer = await _customerRepository.GetMany(customer => customer.AccountId.Equals(id))
                                                .Include(customer => customer.Account)
                                                .FirstOrDefaultAsync();
            if(customer != null)
            {
                customer.FullName = model.FullName ?? customer.FullName;
                customer.Gender = model.Gender ?? customer.Gender;
                customer.Address = model.Address ?? customer.Address;

                if (!string.IsNullOrEmpty(model.OldPassword))
                {
                    if (!PasswordHasher.VerifyPassword(model.OldPassword, customer.Account.PasswordHash))
                    {
                        throw new Exception("Mật khẩu cũ không chính sát.");
                    }
                    if(model.NewPassword != null)
                    {
                        customer.Account.PasswordHash = PasswordHasher.HashPassword(model.NewPassword);
                    }
                }
                _customerRepository.Update(customer);
            }
            else
            {
                throw new Exception("Không tìm thấy customer");
            }
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetCustomer(customer.AccountId) : null!;
        }


        public async Task<CustomerViewModel> UploadAvatar(Guid id, IFormFile image)
        {
            var customer = await _customerRepository.GetMany(customer => customer.AccountId.Equals(id)).FirstOrDefaultAsync();
            if(customer == null)
            {
                throw new Exception("Không tìm thấy customer");
            }

            //xóa hình cũ trong firebase
            if (!string.IsNullOrEmpty(customer.Avatar))
            {
                await _cloudStorageService.Delete(id);
            }

            //upload hình mới
            var url = await _cloudStorageService.Upload(id, image.ContentType, image.OpenReadStream());

            customer.Avatar = url;

            _customerRepository.Update(customer);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetCustomer(customer.AccountId) : null!;
        }

        //CREATE ACCOUNT
        private async Task<Guid> CreateAccount(string phoneNumber, string password)
        {
            //Check phone number
            var existingUser = await _accountRepository.GetMany(account => account.PhoneNumber.Equals(phoneNumber))
                                                        .FirstOrDefaultAsync();
            if (existingUser != null)
            {
                throw new Exception("Số điện thoại đã được sử dụng");
            }

            var accountRole = await _accountRoleRepository.GetMany(role => role.RoleName.Equals(Role.Customer))
                                                            .FirstOrDefaultAsync();
            if (accountRole == null)
            {
                throw new Exception("Không tìm thấy role " + Role.Customer);
            }

            var passwordHash = PasswordHasher.HashPassword(password);

            var id = Guid.NewGuid();

            var account = new Account
            {
                Id = id,
                PhoneNumber = phoneNumber,
                PasswordHash = passwordHash,
                RoleId = accountRole.Id,
                Status = AccountStatus.Pending
            };
            _accountRepository.Add(account);

            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? id : Guid.Empty;
        }


    }
}
