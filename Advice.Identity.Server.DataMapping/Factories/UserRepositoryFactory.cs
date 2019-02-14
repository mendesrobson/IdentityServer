using Advice.Identity.Server.Domain.Interfaces.Factories;
using System;
using System.Collections.Generic;
using System.Text;
using Advice.Identity.Server.Domain.Interfaces.Repositories;
using Advice.Ranoi.Core.Data.Interfaces;

namespace Advice.Identity.Server.DataMapping
{
    public class UserRepositoryFactory : IUserRepositoryFactory
    {
        private IUnitOfWorkFactory UowFactory { get; set; }
        private IContext Context { get; set; }

        public UserRepositoryFactory(IUnitOfWorkFactory uowFactory, IContext context)
        {
            UowFactory = uowFactory;
            Context = context;
        }

        public IUserRepository CreateRepository()
        {
            return new UserRepository(UowFactory.CurrentUnitOfWork(), Context);
        }
    }
}
