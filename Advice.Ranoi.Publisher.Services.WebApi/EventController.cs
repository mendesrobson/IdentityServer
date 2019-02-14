using Advice.Ranoi.Publisher.Services.Facade.Interfaces;
using Advice.Ranoi.Publisher.Services.Facade.Interfaces.Models.Events;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Liera.Publisher.Services.WebApi
{
    [Route("api/[controller]")]
    public class EventController : Controller
    {
        IEventService service;

        public EventController(IEventService service)
        {
            this.service = service;
        }

        [HttpGet]
        public List<EventModel> GetPendingEvents()
        {
            return this.service.GetPendingEvent();
        }

        [HttpPost("{id}")]
        public void RegisterSentEvent(Guid id)
        {
            service.RegisterSentEvent(id);
        }
    }
}
