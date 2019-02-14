using Advice.Ranoi.Core.Data;
using Advice.Identity.Server.Domain;
using Advice.Identity.Server.Domain.Interfaces;
using Advice.Identity.Server.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using Advice.Ranoi.Core.Data.Interfaces;

namespace Advice.Identity.Server.DataMapping
{
    public class UserRepository : Repository<User, IUser>, IUserRepository
    {
        public UserRepository(IUnitOfWork uow, IContext context) : base(uow, context )
        {
        }

        public IUser FindByName(string name)
        {
            throw new NotImplementedException();
        }

        public IUser FindByUserAndPassword(string name, string password)
        {
            throw new NotImplementedException();
        }

        //public IUser FindByName(string name)
        //{
        //    return base.FirstOrDefault(x => x.Name.Equals(name));
        //}

        //public IUser FindByUserAndPassword(string name, string password)
        //{
        //    return base.SingleOrDefault(x => x.Name.Equals(name) && x.Password.Equals(password));
        //}

        public IList<IUser> ListAll()
        {
            return base.ListAll();
        }
    }
}
