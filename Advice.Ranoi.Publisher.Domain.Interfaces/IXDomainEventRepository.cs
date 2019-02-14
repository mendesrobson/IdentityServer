using Advice.Ranoi.Core.Data.Interfaces;
using Advice.Ranoi.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace Advice.Ranoi.Publisher.Domain.Interfaces.Repository
{
    public interface IXDomainEventRepository : IRepository<IXDomainEvent>
    {
        IList<IXDomainEvent> ListPending();
    }
}
