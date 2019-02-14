using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Identity.Server.Services.Facade.Interfaces.Models
{
    public class RefreshTokenRequest
    {
        public String CurrentToken { get; set; }
    }
}
