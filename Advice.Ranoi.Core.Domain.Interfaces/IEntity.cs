using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advice.Ranoi.Core.Domain.Interfaces
{
    public interface IEntity : IPersistable
    {
        Boolean Inactive { get; set; }
        Int64 Version { get; set; }
        Guid TransactionId { get; set; }
        void RegisterObserver(IObserver observer);
    }
}
