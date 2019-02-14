using Advice.Ranoi.Core.Data;
using Advice.Ranoi.Core.Data.Interfaces;
using Advice.Ranoi.Core.Domain.Interfaces;
using Advice.Ranoi.Publisher.Domain;
using Advice.Ranoi.Publisher.Domain.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Advice.Ranoi.Publisher.DataMapping.Repository
{
    public class XDomainEventRepository : Repository<IXDomainEvent, IXDomainEvent>, IXDomainEventRepository
    {
        public XDomainEventRepository(IUnitOfWork uow, IContext context, IObserver observer) : base(uow, context, observer)
        {
        }

        public IList<IXDomainEvent> ListPending()
        {
            return base.Where(Builder.Eq("Published", false)).ToList();
        }
    }
}
