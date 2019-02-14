using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Domain.Interfaces.Factories
{
    public interface IDeclaredFactory<out T>
    {
        T CreateRepository();
        Type GetCreatedType();
    }
}
