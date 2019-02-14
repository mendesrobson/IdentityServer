using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Subscriber.Services.Facade.Interfaces.Models.Notification
{
    public class NotifyRequest
    {
        public Guid Id { get; set; }
        public String Source { get; set; }
        public String Target { get; set; }
        public String Payload { get; set; }
    }
}
