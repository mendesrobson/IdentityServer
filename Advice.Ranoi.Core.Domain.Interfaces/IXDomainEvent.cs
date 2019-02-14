using System;
using System.Collections.Generic;
using System.Text;
using Advice.Ranoi.Core.Domain.Interfaces.Factories;

namespace Advice.Ranoi.Core.Domain.Interfaces
{
    public interface IXDomainEvent : IEntity
    {
        DateTime EventDate { get; }
        void ForcePublish();
        DateTime PublishDate { get; }
        void Publish();
        Boolean Published { get; }
        IPersistable Source { get; }
        String EventType { get; }
        Guid SourceId { get; }
        String Payload { get; }
        String CorrelationId { get; }
    }
}
