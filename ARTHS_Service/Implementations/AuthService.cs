using ARTHS_Data;
using ARTHS_Data.Models.Internal;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Helpers;
using ARTHS_Utility.Settings;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ARTHS_Service.Implementations
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IAccountRepository _accountRepository;

        private readonly AppSetting _appSettings;
        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<AppSetting> appSettings) : base(unitOfWork, mapper)
        {
            _appSettings = appSettings.Value;
            _accountRepository = unitOfWork.Account;
        }

        public async Task<AuthViewModel> AuthenticatedUser(AuthRequest auth)
        {
            var user = await _accountRepository.GetMany(account => account.PhoneNumber.Equals(auth.PhoneNumber))
                                                .Include(account => account.Role)
                                                .FirstOrDefaultAsync();

            if (user != null && PasswordHasher.VerifyPassword(auth.Password, user.PasswordHash))
            {
                var token = GenerateJwtToken(new AuthModel
                {
                    Id = user.Id,
                    Role = user.Role.RoleName
                });

                return new AuthViewModel
                {
                    AccessToken = token
                };
            }

            return null!;
        }

        public async Task<AuthModel?> GetAuthAccount(Guid id)
        {
            var auth = await _accountRepository.GetMany(account => account.Id.Equals(id))
                                                .Include(account => account.Role)
                                                .FirstOrDefaultAsync();
            if (auth != null)
            {
                return new AuthModel
                {
                    Id = auth.Id,
                    Role = auth.Role.RoleName
                };
            }
            return null!;
        }

        public async Task<AccountViewModel?> GetAccount(Guid id)
        {
            return await _accountRepository.GetMany(account => account.Id.Equals(id))
                .ProjectTo<AccountViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }


        //PRIVATE METHOD
        private string GenerateJwtToken(AuthModel auth)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_appSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", auth.Id.ToString()),

                    new Claim("role", auth.Role.ToString()),
                }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
