using JWT;
using Advice.Ranoi.Core.Security.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Security.Domain
{
    public class TokenFactory : ITokenFactory
    {
        String secret = "secret bem grande pra passar no teste";
        IAdviceDateTimeProvider dateTimeProvider;

        public TokenFactory(IAdviceDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
        }

        public IToken CreateToken(Guid userId, string userName)
        {
            return new Token(secret, dateTimeProvider, userId, userName);
        }

        public IToken CreateToken(string token)
        {
            return new Token(secret, dateTimeProvider, token);
        }
    }
}
