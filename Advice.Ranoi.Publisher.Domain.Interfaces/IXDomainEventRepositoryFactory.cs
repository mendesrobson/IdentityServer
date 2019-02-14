using Advice.Ranoi.Publisher.Domain.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Publisher.Domain.Interfaces
{
    public interface IXDomainEventRepositoryFactory
    {
        IXDomainEventRepository CreateRepository();
    }
}
