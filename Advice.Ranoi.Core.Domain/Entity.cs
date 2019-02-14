using Advice.Ranoi.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advice.Ranoi.Core.Domain
{
    public class Entity : IEntity
    {
        public Guid Id { get; private set; }
        public bool Inactive { get; set; }
        public Guid TransactionId { get; set; }
        public long Version { get; set; }
        public long CreatedAt { get; private set; }
        private List<IObserver> Observers { get; set; }

        protected Entity(Guid id)
        {
            Id = id;
            CreatedAt = DateTime.Now.Ticks;
            Observers = new List<IObserver>();
        }

        protected Entity(Guid id, IObserver observer) : this(id)
        {
            RegisterObserver(observer);
        }

        protected void Notify(IXDomainEvent evt)
        {
            Observers.ForEach(x => x.Update(evt));
        }

        public void RegisterObserver(IObserver observer)
        {
            if (Observers == null)
                Observers = new List<IObserver>();

            Observers.Add(observer);
        }
    }
}
