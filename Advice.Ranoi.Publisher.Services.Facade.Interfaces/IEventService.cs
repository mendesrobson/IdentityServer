using Advice.Ranoi.Publisher.Services.Facade.Interfaces.Models.Events;
using System;
using System.Collections.Generic;

namespace Advice.Ranoi.Publisher.Services.Facade.Interfaces
{
    public interface IEventService
    {
        List<EventModel> GetPendingEvent();
        void RegisterSentEvent(Guid idEvent);
    }
}
