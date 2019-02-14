using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advice.Ranoi.Core.Domain.Interfaces
{
    public interface IPersistable
    {
        Guid Id { get; }
        Int64 CreatedAt { get; }
    }
}
