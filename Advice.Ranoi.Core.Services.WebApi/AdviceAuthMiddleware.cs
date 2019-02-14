using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Advice.Ranoi.Core.Services.WebApi
{
    public class AdviceAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMustAuth _ignore;

        public AdviceAuthMiddleware(RequestDelegate next, IMustAuth ignore)
        {
            _next = next;
            _ignore = ignore;
        }

        private bool Auth(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (String.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return false;

            var tokenStr = authHeader.Replace("Bearer ", "");

            var tokenHandler = new JwtSecurityTokenHandler();

            var sharedKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Aqui e o segret quando maior melhor para validar , se preferir posso colocar batatinhas aqui srsrrs"));

            var validationParameters = new TokenValidationParameters();
            validationParameters.IssuerSigningKey = sharedKey;
            validationParameters.ValidateAudience = false;
            validationParameters.ValidAudience = "http://www.advicesystem.com.br";
            validationParameters.ValidateIssuer = false;
            validationParameters.ValidIssuer = "self";
            validationParameters.ValidateLifetime = false;

            try
            {
                SecurityToken token = new JwtSecurityToken();

                ClaimsPrincipal principal = tokenHandler.ValidateToken(tokenStr, validationParameters, out token);
                context.User = principal;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!_ignore.MustAuth(context) || Auth(context))
                await _next(context);
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
        }
    }

    public static class AdviceAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAdviceAuth(this IApplicationBuilder app, IMustAuth ignore)
        {
            return app.UseMiddleware<AdviceAuthMiddleware>(ignore);
        }
    }
}
