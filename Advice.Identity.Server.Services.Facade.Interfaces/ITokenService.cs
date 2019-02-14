using Advice.Identity.Server.Services.Facade.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Identity.Server.Services.Facade.Interfaces
{
    public interface ITokenService
    {
        String GetToken(GetTokenRequest request);
        String RefreshToken(RefreshTokenRequest request);

    }
}
