using Advice.Ranoi.Subscriber.Services.Facade.Interfaces.Models.Notification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Subscriber.Services.Facade.Interfaces
{
    public interface INotificationService
    {
        Boolean Notify(NotifyRequest request);
    }
}
