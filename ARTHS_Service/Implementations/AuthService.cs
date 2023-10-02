using ARTHS_Data;
using ARTHS_Data.Models.Internal;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Exceptions;
using ARTHS_Utility.Helpers;
using ARTHS_Utility.Settings;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
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

        public async Task<TokenViewModel> Authenticated(AuthRequest auth)
        {
            var account = await _accountRepository.GetMany(account => account.PhoneNumber.Equals(auth.PhoneNumber))
                                                .Include(account => account.Role)
                                                .FirstOrDefaultAsync();

            if (account != null && PasswordHasher.VerifyPassword(auth.Password, account.PasswordHash))
            {
                var accessToken = GenerateJwtToken(new AuthModel
                {
                    Id = account.Id,
                    Role = account.Role.RoleName,
                    Status = account.Status
                });
                return new TokenViewModel
                {
                    AccessToken = accessToken
                };
            }
            throw new NotFoundException("Sai tài khoản hoặc mật khẩu.");
        }

        public async Task<TokenViewModel> RefreshAuthentication(string currentToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_appSettings.SecretKey);
                tokenHandler.ValidateToken(currentToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                
                if(jwtToken.ValidTo > DateTime.Now)
                {
                    //trả về token cũ nếu chưa expire
                    return new TokenViewModel { AccessToken = currentToken };
                }
                var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
                var user = await GetAuth(userId);
                return new TokenViewModel
                {
                    AccessToken = GenerateJwtToken(user!)
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        //public async Task<TokenViewModel> RefreshAuthentication(RefreshTokenModel model)
        //{
        //    var account = await _accountRepository.GetMany(account => account.RefreshToken!.Equals(model.refreshToken)).Include(account => account.Role).FirstOrDefaultAsync();
        //    if(account == null)
        //    {
        //        throw new NotFoundException("Không tìm thấy account");
        //    }
        //    if (!IsRefreshTokenValid(model.refreshToken))
        //    {
        //        throw new InvalidRefreshTokenException("Refresh token đã hết hạn");
        //    }

        //    var newAccessToken = GenerateJwtToken(new AuthModel
        //    {
        //        Id = account.Id,
        //        Role = account.Role.RoleName,
        //        Status = account.Status
        //    });
        //    var newRefreshToken = GenerateRefreshToken();
        //    account.RefreshToken = newRefreshToken;
        //    _accountRepository.Update(account);

        //    if (await _unitOfWork.SaveChanges() > 0)
        //    {
        //        return new TokenViewModel
        //        {
        //            AccessToken = newAccessToken,
        //            RefreshToken = newRefreshToken
        //        };
        //    }
        //    return null!;
        //}

        public async Task<AuthModel> GetAuth(Guid id)
        {
            var auth = await _accountRepository.GetMany(account => account.Id.Equals(id))
                                                .Include(account => account.Role)
                                                .FirstOrDefaultAsync();
            if (auth != null)
            {
                return new AuthModel
                {
                    Id = auth.Id,
                    Role = auth.Role.RoleName,
                    Status = auth.Status
                };
            }
            throw new NotFoundException("Không tìm thấy account.");
        }


        public async Task<AccountViewModel> GetAccount(Guid id)
        {
            return await _accountRepository.GetMany(account => account.Id.Equals(id))
                .ProjectTo<AccountViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy account.");
        }

        //---------------------------------------------------------------------------------------
        //PRIVATE METHOD
        //private bool IsRefreshTokenValid(string refreshToken)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.UTF8.GetBytes(_appSettings.RefreshTokenSecret);

        //    try
        //    {
        //        var tokenParams = new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ValidateIssuer = false,
        //            ValidateAudience = false,
        //            ValidateLifetime = true,
        //            ClockSkew = TimeSpan.Zero
        //        };

        //        tokenHandler.ValidateToken(refreshToken, tokenParams, out SecurityToken validatedToken);

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}



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

                    new Claim("status", auth.Status.ToString()),
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        //private string GenerateRefreshToken()
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.UTF8.GetBytes(_appSettings.RefreshTokenSecret);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[]
        //        {
        //            new Claim("type", "refreshToken"),
        //        }),
        //        Expires = DateTime.Now.AddDays(7),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}
    }
}
