using Advice.Ranoi.Core.Data.Interfaces;
using Advice.Ranoi.Core.Domain.Interfaces;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Advice.Ranoi.Core.Data
{
    public class Transaction : ITransaction
    {
        public Guid ParentId { get; private set; }
        public Guid Id { get; private set; }
        public long CreatedAt { get; private set; }
        public bool Inactive { get; set; }
        public long Version { get; set; }
        public Guid TransactionId { get; set; }
        [BsonIgnore]
        private ITransaction Next { get; set; }
        [BsonElement]
        private Dictionary<Guid, IEntity> toAdd { get; set; }
        [BsonElement]
        private Dictionary<Guid, IEntity> toSave { get; set; }
        [BsonElement]
        private Dictionary<Guid, IEntity> toRemove { get; set; }
        [BsonElement]
        private Dictionary<Guid, IEntity> rollback { get; set; }
        [BsonIgnore]
        public Dictionary<Guid, IEntity> ToAdd
        {
            get
            {
                Dictionary<Guid, IEntity> fullList = new Dictionary<Guid, IEntity>();

                this.FullChain.Cast<Transaction>().SelectMany(x => x.toAdd).ToList().ForEach(x =>
                {
                    fullList.Add(x.Key, x.Value);
                });

                return fullList;
            }
        }
        [BsonIgnore]
        public Dictionary<Guid, IEntity> ToSave
        {
            get
            {
                Dictionary<Guid, IEntity> fullList = new Dictionary<Guid, IEntity>();

                this.FullChain.Cast<Transaction>().SelectMany(x => x.toSave).ToList().ForEach(x =>
                {
                    fullList.Add(x.Key, x.Value);
                });

                return fullList;
            }
        }
        [BsonIgnore]
        public Dictionary<Guid, IEntity> ToRemove
        {
            get
            {
                Dictionary<Guid, IEntity> fullList = new Dictionary<Guid, IEntity>();

                this.FullChain.Cast<Transaction>().SelectMany(x => x.toRemove).ToList().ForEach(x =>
                {
                    fullList.Add(x.Key, x.Value);
                });

                return fullList;
            }
        }
        [BsonIgnore]
        public Dictionary<Guid, IEntity> Rollback
        {
            get
            {
                Dictionary<Guid, IEntity> fullList = new Dictionary<Guid, IEntity>();

                this.FullChain.Cast<Transaction>().SelectMany(x => x.rollback).ToList().ForEach(x =>
                {
                    fullList.Add(x.Key, x.Value);
                });

                return fullList;
            }
        }

        private Int32 TotalEntities { get { return toAdd.Count + toSave.Count + toRemove.Count + rollback.Count; } }

        public Boolean IsComplete { get; private set; }

        public IList<ITransaction> FullChain
        {
            get
            {
                List<ITransaction> chain = new List<ITransaction>();
                chain.Add(this);

                Transaction next = (Transaction)this.Next;

                while (next != null)
                {
                    chain.Add(next);
                    next = (Transaction)next.Next;
                }

                return chain;
            }
        }

        public Transaction()
        {
            this.Id = Guid.NewGuid();
            this.ParentId = this.Id;
            this.CreatedAt = DateTime.Now.Ticks;

            this.toAdd = new Dictionary<Guid, IEntity>();
            this.toSave = new Dictionary<Guid, IEntity>();
            this.toRemove = new Dictionary<Guid, IEntity>();
            this.rollback = new Dictionary<Guid, IEntity>();
        }

        public Transaction(Guid parentId) : this()
        {
            this.ParentId = parentId;
        }

        public void RegisterRollback(IEntity entity)
        {
            if (!rollback.ContainsKey(entity.Id))
            {
                if (TotalEntities < 50)
                    rollback.Add(entity.Id, entity);
                else
                    GetNext().RegisterRollback(entity);
            }
            else
                rollback[entity.Id] = entity;
        }

        public void RegisterSave(IEntity entity)
        {
            if (rollback.ContainsKey(entity.Id))
            {
                toSave.Add(entity.Id, entity);
            }
            else
            {
                if (Next != null)
                {
                    GetNext().RegisterSave(entity);
                }
                else
                {
                    if (TotalEntities < 50)
                        toAdd.Add(entity.Id, entity);
                    else
                        GetNext().RegisterSave(entity);
                }
            }
        }

        public void RegisterRemove(IEntity entity)
        {
            if (rollback.ContainsKey(entity.Id) && !toRemove.ContainsKey(entity.Id))
            {
                if (TotalEntities < 50)
                    toRemove.Add(entity.Id, entity);
                else
                    GetNext().RegisterRemove(entity);
            }
        }

        public void RegisterObserver(IObserver observer)
        {
        }

        private ITransaction GetNext()
        {
            if (Next == null)
                Next = new Transaction(this.ParentId);

            return Next;
        }

        public void Complete()
        {
            this.IsComplete = true;

            if (Next != null)
                Next.Complete();
        }
    }
}
