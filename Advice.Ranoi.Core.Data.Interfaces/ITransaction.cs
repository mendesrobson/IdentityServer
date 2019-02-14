using Advice.Ranoi.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Data.Interfaces
{
    public interface ITransaction : IEntity
    {
        Guid ParentId { get; }
        Boolean IsComplete { get; }
        Dictionary<Guid, IEntity> Rollback { get; }
        Dictionary<Guid, IEntity> ToAdd { get; }
        Dictionary<Guid, IEntity> ToRemove { get; }
        Dictionary<Guid, IEntity> ToSave { get; }
        void RegisterRemove(IEntity entity);
        void RegisterRollback(IEntity entity);
        void RegisterSave(IEntity entity);
        void Complete();
        IList<ITransaction> FullChain { get; }
    }
}
