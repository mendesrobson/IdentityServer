using JWT;
using Advice.Ranoi.Core.Security.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Security.Domain
{
    public class AdviceDateTimeProvider : IAdviceDateTimeProvider
    {
        public DateTimeOffset GetNow()
        {
            var provider = new UtcDateTimeProvider();
            return provider.GetNow();
        }
    }
}
