using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Domain.Interfaces.Factories
{
    public interface IGenericRepositoryFactory
    {
        T CreateRepository<T>();
    }
}
