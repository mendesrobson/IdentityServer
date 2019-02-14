using Advice.Ranoi.Core.Data.Interfaces;
using Advice.Ranoi.Core.Domain.Interfaces;
using Advice.Ranoi.Publisher.DataMapping.Repository;
using Advice.Ranoi.Publisher.Domain.Interfaces;
using Advice.Ranoi.Publisher.Domain.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Publisher.DataMapping.Factory
{
    public class XDomainEventRepositoryFactory : IXDomainEventRepositoryFactory
    {
        private IUnitOfWorkFactory UowFactory { get; set; }
        private IContext Context { get; set; }
        private IObserver Observer { get; set; }

        public XDomainEventRepositoryFactory(IUnitOfWorkFactory UowFactory, IContext Context, IObserver observer)
        {
            this.UowFactory = UowFactory;
            this.Context = Context;
            this.Observer = observer;
        }

        public IXDomainEventRepository CreateRepository()
        {
            return new XDomainEventRepository(UowFactory.CurrentUnitOfWork(), Context, Observer);
        }
    }
}
