using Advice.Identity.Server.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Identity.Server.Domain.Interfaces.Factories
{
    public interface IUserRepositoryFactory
    {
        IUserRepository CreateRepository();
    }
}
