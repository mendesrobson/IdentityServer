using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Security.Domain.Interfaces
{
    public interface IJWTIgnoreProvider
    {
        Boolean Authenticate(String url, String path, String method);

    }
}
