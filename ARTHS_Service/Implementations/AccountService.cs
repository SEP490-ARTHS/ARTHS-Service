using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Filters;
using ARTHS_Data.Models.Requests.Get;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Constants;
using ARTHS_Utility.Exceptions;
using ARTHS_Utility.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Service.Implementations
{
    public class AccountService : BaseService, IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountRoleRepository _accountRoleRepository;
        public AccountService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _accountRepository = unitOfWork.Account;
            _accountRoleRepository = unitOfWork.AccountRole;
        }

        public async Task<ListViewModel<AccountViewModel>> GetAccounts(AccountFilterModel filter, PaginationRequestModel pagination)
        {
            var query = _accountRepository.GetAll();
            if (!string.IsNullOrEmpty(filter.FullName))
            {
                query = query.Where(account => (account.CustomerAccount != null && account.CustomerAccount.FullName.Contains(filter.FullName)) ||
                                               (account.OwnerAccount != null && account.OwnerAccount.FullName.Contains(filter.FullName)) ||
                                               (account.TellerAccount != null && account.TellerAccount.FullName.Contains(filter.FullName)) ||
                                               (account.StaffAccount != null && account.StaffAccount.FullName.Contains(filter.FullName)));
            }
            if (!string.IsNullOrEmpty(filter.PhoneNumber))
            {
                query = query.Where(account => account.PhoneNumber.Contains(filter.PhoneNumber));
            }
            var listAccount = query
                .ProjectTo<AccountViewModel>(_mapper.ConfigurationProvider)
                .OrderByDescending(account => account.CreateAt);
            var accounts = await listAccount.Skip(pagination.PageNumber * pagination.PageSize).Take(pagination.PageSize).AsNoTracking().ToListAsync();
            var totalRow = await listAccount.AsNoTracking().CountAsync();
            if(accounts != null || accounts != null && accounts.Any())
            {
                return new ListViewModel<AccountViewModel>
                {
                    Pagination = new PaginationViewModel
                    {
                        PageNumber = pagination.PageNumber,
                        PageSize = pagination.PageSize,
                        TotalRow = totalRow
                    },
                    Data = accounts
                };
            }
            return null!;
        }

        public async Task<List<AccountViewModel>> GetCustomers(AccountFilterModel filter)
        {
            var customerAccountQuery = _accountRepository.GetAll().Where(account => account.CustomerAccount != null);
            if (!string.IsNullOrEmpty(filter.FullName))
            {
                customerAccountQuery = customerAccountQuery.Where(account => account.CustomerAccount != null && account.CustomerAccount.FullName.Contains(filter.FullName));
            }
            if (!string.IsNullOrEmpty(filter.PhoneNumber))
            {
                customerAccountQuery = customerAccountQuery.Where(account => account.PhoneNumber.Contains(filter.PhoneNumber));
            }

            return await customerAccountQuery
                .ProjectTo<AccountViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<AccountViewModel>> GetStaffs(AccountFilterModel filter)
        {
            // Chỉ lấy staff
            var staffAccountsQuery = _accountRepository.GetAll().Where(account => account.StaffAccount != null);
            if (!string.IsNullOrEmpty(filter.FullName))
            {
                staffAccountsQuery = staffAccountsQuery.Where(account => account.StaffAccount!.FullName.Contains(filter.FullName));
            }
            if (!string.IsNullOrEmpty(filter.PhoneNumber))
            {
                staffAccountsQuery = staffAccountsQuery.Where(account => account.PhoneNumber.Contains(filter.PhoneNumber));
            }

            return await staffAccountsQuery
                .ProjectTo<AccountViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<AccountViewModel>> GetOwners(AccountFilterModel filter)
        {
            //Chỉ lấy owner
            var ownerAccountsQuery = _accountRepository.GetAll().Where(account => account.OwnerAccount != null);
            if (!string.IsNullOrEmpty(filter.FullName))
            {
                ownerAccountsQuery = ownerAccountsQuery.Where(account => account.OwnerAccount!.FullName.Contains(filter.FullName));
            }
            if (!string.IsNullOrEmpty(filter.PhoneNumber))
            {
                ownerAccountsQuery = ownerAccountsQuery.Where(account => account.PhoneNumber.Contains(filter.PhoneNumber));
            }

            return await ownerAccountsQuery
                .ProjectTo<AccountViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<AccountViewModel>> GetTellers(AccountFilterModel filter)
        {
            // Chỉ lấy teller
            var tellerAccountsQuery = _accountRepository.GetAll().Where(account => account.TellerAccount != null);
            if (!string.IsNullOrEmpty(filter.FullName))
            {
                tellerAccountsQuery = tellerAccountsQuery.Where(account => account.TellerAccount!.FullName.Contains(filter.FullName));
            }
            if (!string.IsNullOrEmpty(filter.PhoneNumber))
            {
                tellerAccountsQuery = tellerAccountsQuery.Where(account => account.PhoneNumber.Contains(filter.PhoneNumber));
            }

            return await tellerAccountsQuery
                .ProjectTo<AccountViewModel>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }


        //CREATE ACCOUNT
        public async Task<Guid> CreateAccount(string phoneNumber, string password, string role)
        {
            //Check phone number
            var existingUser = await _accountRepository.GetMany(account => account.PhoneNumber.Equals(phoneNumber))
                                                        .FirstOrDefaultAsync();
            if (existingUser != null)
            {
                throw new ConflictException("Số điện thoại đã được sử dụng");
            }

            var accountRole = await _accountRoleRepository.GetMany(ro => ro.RoleName.Equals(role))
                                                            .FirstOrDefaultAsync();
            if (accountRole == null)
            {
                throw new NotFoundException("Không tìm thấy role " + role);
            }

            var passwordHash = PasswordHasher.HashPassword(password);

            var id = Guid.NewGuid();

            var account = new Account
            {
                Id = id,
                PhoneNumber = phoneNumber,
                PasswordHash = passwordHash,
                RoleId = accountRole.Id,
                Status = UserStatus.Pending
            };
            _accountRepository.Add(account);

            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? id : Guid.Empty;
        }
    }
}
