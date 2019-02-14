using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Security.Domain.Interfaces
{
    public interface ITokenFactory
    {
        IToken CreateToken(Guid userId, String userName);
        IToken CreateToken(String token);
    }
}
