using Advice.Ranoi.Core.Domain.Interfaces;
using Advice.Ranoi.Core.Domain.Interfaces.Factories;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Advice.Ranoi.Publisher.Domain
{
    public class XDomainEvent : IXDomainEvent
    {
        public DateTime EventDate { get; private set; }

        public DateTime PublishDate { get; private set; }

        public bool Published { get; private set; }

        public IPersistable Source { get; private set; }

        public Guid SourceId { get; private set; }

        public string Payload { get; protected set; }

        public string CorrelationId { get; protected set; }

        public bool Inactive { get; set; }
        public long Version { get; set; }
        public Guid TransactionId { get; set; }

        [BsonId]
        public Guid Id { get; private set; }

        public long CreatedAt { get; set; }

        public string EventType { get; private set; }

        protected XDomainEvent(IEntity source)
        {
            Id = Guid.NewGuid();
            EventDate = DateTime.Now;
            SourceId = source.Id;
        }

        public void ForcePublish()
        {
            this.Published = true;
        }

        //public void GerarXML(IGenericRepositoryFactory genericFactory)
        //{
        //    throw new NotImplementedException();
        //}

        public void Publish()
        {
            this.Published = true;
            this.PublishDate = DateTime.Now;
        }

        public void RegisterObserver(IObserver observer)
        {
        }
    }
}
