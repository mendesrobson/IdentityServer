using Advice.Ranoi.Core.Data.Interfaces;
using Advice.Ranoi.Core.Domain.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Advice.Ranoi.Core.Data
{
    public class Repository<T, IT> : IEntityRepository<IT>
        where T : IT
        where IT : IEntity
    {
        private IUnitOfWork UnitOfWork { get; set; }
        private IContext Context { get; set; }
        private IObserver Observer { get; set; }
        protected FilterDefinitionBuilder<T> Builder { get { return MongoDB.Driver.Builders<T>.Filter; } }

        protected IMongoCollection<T> Collection
        {
            get
            {
                return Context.DataBase.GetCollection<T>(Context.FindCollectionName(typeof(T)));
            }
        }

        public Repository(IUnitOfWork uow, IContext context, IObserver observer)
        {
            Context = context;
            UnitOfWork = uow;
            Observer = observer;
        }

        public Repository(IUnitOfWork uow, IContext context)
        {
            Context = context;
            UnitOfWork = uow;
        }

        private void AttachObserver(IEntity entity)
        {
            entity.RegisterObserver(Observer);
        }

        public IT Get(Guid id)
        {
            var query = Builder.Eq("_id", id);

            var entity = Collection.Find(query).SingleOrDefault();

            if (entity != null)
            {
                AttachObserver(entity);
                UnitOfWork.RegisterLoaded(entity);
            }

            return entity;
        }

        public IT SingleOrDefault(FilterDefinition<T> filter)
        {
            var query = Builder.And(filter, Builder.Eq("Inactive", false));

            var entity = Collection.Find(query).SingleOrDefault();

            if (entity != null)
            {
                AttachObserver(entity);
                UnitOfWork.RegisterLoaded(entity);
            }

            return entity;
        }

        public IT Single(FilterDefinition<T> filter)
        {
            IT entity = SingleOrDefault(filter);

            if (entity == null)
                throw new Exception("Not found");

            return entity;
        }

        public IT FirstOrDefault(FilterDefinition<T> filter)
        {
            var query = Builder.And(filter, Builder.Eq("Inactive", false));

            var entity = Collection.Find(query).FirstOrDefault();

            if (entity != null)
            {
                AttachObserver(entity);
                UnitOfWork.RegisterLoaded(entity);
            }

            return entity;
        }

        public IT First(FilterDefinition<T> filter)
        {
            IT entity = FirstOrDefault(filter);

            if (entity == null)
                throw new Exception("Not found");

            return entity;
        }

        public IList<IT> Where(FilterDefinition<T> filter)
        {
            var query = Builder.And(filter, Builder.Eq("Inactive", false));

            var entities = Collection.Find(query).ToList<T>().Cast<IT>().ToList();

            if (entities != null)
            {
                entities.ForEach(x => AttachObserver(x));
                entities.ForEach(x => UnitOfWork.RegisterLoaded(x));
            }

            return entities;
        }

        public IList<IT> ListAll()
        {
            var query = Builder.Eq("Inactive", false);

            var entities = Collection.Find(query).ToList<T>().Cast<IT>().ToList();

            if (entities != null)
            {
                entities.ForEach(x => AttachObserver(x));
                entities.ForEach(x => UnitOfWork.RegisterLoaded(x));
            }

            return entities;
        }

        public Int32 Count(FilterDefinition<T> filter)
        {
            var query = Builder.And(filter, Builder.Eq("Inactive", false));

            return Collection.Find(query).ToList<T>().Count;
        }

        public Int32 CountAll()
        {
            var query = Builder.Eq("Inactive", false);

            return Collection.Find(query).ToList<T>().Count;
        }

        #region Legacy

        private IT SingleOrDefault(Expression<Func<T, bool>> expression)
        {
            ISpecification<T> spec = new Specification<T>(expression);

            spec = spec.And(new Specification<T>(x => x.Inactive.Equals(false)));

            var entity = Collection.AsQueryable<T>().SingleOrDefault(spec.Predicate);

            if (entity != null)
            {
                AttachObserver(entity);
                UnitOfWork.RegisterLoaded(entity);
            }

            return entity;
        }

        private IT Single(Expression<Func<T, bool>> expression)
        {
            IT entity = SingleOrDefault(expression);

            if (entity == null)
                throw new Exception("Not found");

            return entity;
        }

        private IT FirstOrDefault(Expression<Func<T, bool>> expression)
        {
            ISpecification<T> spec = new Specification<T>(expression);

            spec = spec.And(new Specification<T>(x => x.Inactive.Equals(false)));

            var entity = Collection.AsQueryable<T>().FirstOrDefault(spec.Predicate);

            if (entity != null)
            {
                AttachObserver(entity);
                UnitOfWork.RegisterLoaded(entity);
            }

            return entity;
        }

        private IT First(Expression<Func<T, bool>> expression)
        {
            IT entity = FirstOrDefault(expression);

            if (entity == null)
                throw new Exception("Not found");

            return entity;
        }

        private IList<IT> Where(Expression<Func<T, bool>> expression)
        {
            ISpecification<T> spec = new Specification<T>(expression);

            spec = spec.And(new Specification<T>(x => x.Inactive.Equals(false)));

            var entities = Collection.AsQueryable<T>().Where(spec.Predicate).ToList<T>().Cast<IT>().ToList();

            if (entities != null)
            {
                entities.ForEach(x => AttachObserver(x));
                entities.ForEach(x => UnitOfWork.RegisterLoaded(x));
            }

            return entities;
        }

        private IList<IT> ListAllLegacy()
        {
            var entities = Collection.AsQueryable<T>().Where(x => x.Inactive.Equals(false)).ToList<T>().Cast<IT>().ToList();

            entities.ForEach(x => AttachObserver(x));

            if (entities != null)
                entities.ForEach(x => UnitOfWork.RegisterLoaded(x));

            return entities;
        }


        #endregion

    }
}
