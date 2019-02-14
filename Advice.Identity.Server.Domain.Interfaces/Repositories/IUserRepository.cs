using Advice.Ranoi.Core.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Identity.Server.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<IUser>
    {
        IUser FindByName(String name);
        IUser FindByUserAndPassword(String name, String password);
        IList<IUser> ListAll();
    }
}
