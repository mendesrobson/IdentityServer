using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Security.Domain.Interfaces
{
    public interface IToken
    {
        string Error { get; set; }
        DateTimeOffset ExpiresAt { get; }
        DateTimeOffset IssuedAt { get; }
        string Issuer { get; }
        Guid UserId { get; }
        string UserName { get; }
        bool Valid { get; set; }

        string GetToken();
        void SetClaim(String name, String value);
        String GetClaim(String name);
    }
}
