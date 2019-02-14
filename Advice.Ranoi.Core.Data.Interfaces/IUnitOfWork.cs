using Advice.Ranoi.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advice.Ranoi.Core.Data.Interfaces
{
    public interface IUnitOfWork
    {
        void RegisterLoaded(IEntity entity);

        Boolean IsCommited { get; }
        void Save(IEntity entity);
        void Delete(IEntity entity);
        void Commit();
    }
}
