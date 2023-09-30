﻿using ARTHS_Data.Models.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using ARTHS_Utility.Exceptions;

namespace ARTHS_API.Configurations.Middleware
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public ICollection<string> Roles { get; set; }

        public AuthorizeAttribute(params string[] roles)
        {
            Roles = roles.Select(x => x.ToLower()).ToList();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var auth = (AuthModel?)context.HttpContext.Items["User"];
            if (auth == null)
            {
                throw new UnauthorizedException("Unauthorized");
            }
            else
            {
                var role = auth.Role;
                var isValid = false;
                if (Roles.Contains(role.ToLower()))
                {
                    isValid = true;
                }
                if (!isValid)
                {
                    throw new ForbiddenException("Forbidden");
                }
            }
        }
    }
}
