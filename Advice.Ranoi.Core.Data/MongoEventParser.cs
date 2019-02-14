using Advice.Ranoi.Core.Data.Interfaces;
using Advice.Ranoi.Core.Domain.Interfaces;
using Advice.Ranoi.Core.Domain.Interfaces.Factories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Data
{
    public class MongoEventParser : IObserver
    {
        IUnitOfWorkFactory uowFactory;

        public MongoEventParser(IUnitOfWorkFactory uowFactory)
        {
            this.uowFactory = uowFactory;
        }

        public void Update(IXDomainEvent evt)
        {
            var uow = uowFactory.CurrentUnitOfWork();
            uow.Save(evt);
        }
    }
}
