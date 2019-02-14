using Advice.Ranoi.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advice.Ranoi.Core.Data.Interfaces
{
    public interface IRepository<out T> where T : IPersistable
    {
        T Get(Guid id);
    }
}
