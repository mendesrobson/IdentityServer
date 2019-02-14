using Advice.Identity.Server.Domain.Interfaces.Factories;
using System;
using System.Collections.Generic;
using System.Text;
using Advice.Identity.Server.Domain.Interfaces;

namespace Advice.Identity.Server.Domain.Factories
{
    public class UserFactory : IUserFactory
    {
        public IUser CreateUser(string name, string password)
        {
            return new User(name, password);
        }
    }
}
