using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Domain.Interfaces
{
    public class RootAttribute : Attribute
    {
        public Type _rootType;

        public RootAttribute(Type rootType)
        {
            _rootType = rootType;
        }
    }
}
