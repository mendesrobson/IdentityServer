using Advice.Ranoi.Core.Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advice.Ranoi.Core.Data.Interfaces
{
    public interface IContext
    {
        IMongoDatabase DataBase { get; }
        String FindCollectionName(Type entityType);
        IMongoCollection<BsonDocument> GetBsonCollection(Type entityType);
        Boolean Add(IEntity entity);
        Boolean Update(IEntity entity, Guid transactionId);
        Boolean Remove(IEntity entity, Guid transactionId);
        Boolean Purge(IEntity entity);
        Boolean Lock(IEntity entity, Guid transactionId);
        Boolean UnLock(IEntity entity, Guid transactionId);

    }
}
