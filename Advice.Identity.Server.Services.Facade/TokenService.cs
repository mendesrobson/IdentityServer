using Advice.Identity.Server.Services.Facade.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Advice.Identity.Server.Services.Facade.Interfaces.Models;
using Advice.Ranoi.Core.Security.Domain.Interfaces;
using Advice.Identity.Server.Domain.Interfaces.Factories;
using Advice.Ranoi.Core.Data.Interfaces;

namespace Advice.Identity.Server.Services.Facade
{
    public class TokenService : ITokenService
    {
        ITokenFactory tokenFactory;
        IUserFactory userFactory;
        IUnitOfWorkFactory unitOfWorkFactory;
        IUserRepositoryFactory userRepositoryFactory;

        public TokenService(ITokenFactory tokenFactory, IUserFactory userFactory, IUnitOfWorkFactory unitOfWorkFactory, IUserRepositoryFactory userRepositoryFactory)
        {
            this.tokenFactory = tokenFactory;
            this.userFactory = userFactory;
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.userRepositoryFactory = userRepositoryFactory;
        }

        public string GetToken(GetTokenRequest request)
        {
            var userRepository = userRepositoryFactory.CreateRepository();
            var user = userRepository.FindByName(request.Login);

            if (user != null && user.Password.Equals(request.Password))
            {
                var token = this.tokenFactory.CreateToken(user.Id, request.Login);
                return token.GetToken();
            }
            return String.Empty;
        }

        public string RefreshToken(RefreshTokenRequest request)
        {
            var expiredToken = tokenFactory.CreateToken(request.CurrentToken);

            if (!expiredToken.Valid && expiredToken.Error.Equals("Token Expired"))
            {
                var newToken = tokenFactory.CreateToken(expiredToken.UserId, expiredToken.UserName);
                return newToken.GetToken();
            }

            return String.Empty;
        }
    }
}
