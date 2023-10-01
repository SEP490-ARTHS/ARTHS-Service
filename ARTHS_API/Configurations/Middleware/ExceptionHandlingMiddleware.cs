﻿using ARTHS_Utility.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace ARTHS_API.Configurations.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            if (exception is PhoneNumberAlreadyExistsException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            }
            else if (exception is RoleNotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            else if (exception is UnauthorizedException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else if (exception is ForbiddenException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
            else if(exception is InvalidOldPasswordException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else if(exception is AccountNotFoundException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            var result = JsonConvert.SerializeObject(new { message = exception.Message });
            return context.Response.WriteAsync(result);
        }
    }
}