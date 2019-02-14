using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Identity.Server.Services.Facade.Interfaces.Models
{
    public class GetTokenRequest
    {
        public String Login { get; set; }
        public String Password { get; set; }
    }
}
