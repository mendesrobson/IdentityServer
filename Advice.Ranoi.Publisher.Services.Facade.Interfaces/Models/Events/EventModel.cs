using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Publisher.Services.Facade.Interfaces.Models.Events
{
    public class EventModel
    {
        public Guid EventId { get; set; }
        public string Publisher { get; set; }
        public string Topic { get; set; }
        public string Payload { get; set; }
    }
}
