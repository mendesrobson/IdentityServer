using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Domain.Interfaces
{
    public interface IXDomainEventParser
    {
        void Parse<T>(T e) where T : IXDomainEvent; //Analizar
    }
}
