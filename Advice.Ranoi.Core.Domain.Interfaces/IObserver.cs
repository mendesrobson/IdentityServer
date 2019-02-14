using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Domain.Interfaces
{
    public interface IObserver
    {
        void Update(IXDomainEvent evt);
    }
}
