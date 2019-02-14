using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Services.WebApi
{
    public interface IMustAuth
    {
        Boolean MustAuth(HttpContext context);
    }

    public class IgnoreAll : IMustAuth
    {
        public bool MustAuth(HttpContext context)
        {
            return false;
        }
    }

    public class StandardAuth : IMustAuth
    {
        public bool MustAuth(HttpContext context)
        {
            if (context.Request.Method.ToUpper().Equals("OPTIONS"))
                return false;

            return true;
        }
    }
}
