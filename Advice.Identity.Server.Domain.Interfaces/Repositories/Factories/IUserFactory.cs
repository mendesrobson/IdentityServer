using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Identity.Server.Domain.Interfaces.Factories
{
    public interface IUserFactory
    {
        IUser CreateUser(String name, String password);
    }
}
