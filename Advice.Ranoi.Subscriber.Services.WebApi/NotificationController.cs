using Advice.Ranoi.Subscriber.Services.Facade.Interfaces;
using Advice.Ranoi.Subscriber.Services.Facade.Interfaces.Models.Notification;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Subscriber.Services.WebApi
{
    [Route("api/[controller]")]
    public class NotificationController : Controller
    {
        INotificationService service;

        public NotificationController(INotificationService service)
        {
            this.service = service;
        }

        public bool Notify([FromBody] NotifyRequest request)
        {
            return service.Notify(request);
        }
    }
}
