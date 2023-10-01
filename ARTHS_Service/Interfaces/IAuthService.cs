﻿using ARTHS_Data.Models.Internal;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Views;

namespace ARTHS_Service.Interfaces
{
    public interface IAuthService
    {
        Task<TokenViewModel> Authenticated(AuthRequest auth);
        Task<AuthModel?> GetAuthAccount(Guid id);
        Task<TokenViewModel> RefreshAuthentication(RefreshTokenModel model);
    }
}
