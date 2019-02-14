using Advice.Ranoi.Core.Data.Interfaces;
using Advice.Ranoi.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;

namespace Advice.Ranoi.Core.Data
{
    public class Specification<T> : ISpecification<T> where T : IEntity
    {
        public Expression<Func<T, bool>> Predicate { get; protected set; }

        public Specification()
        {
            this.Predicate = T => true;
        }

        public Specification(Expression<Func<T, bool>> predicate)
        {
            Predicate = predicate;
        }

        public ISpecification<T> And(ISpecification<T> specification)
        {
            return new Specification<T>(this.Predicate.And(specification.Predicate));
        }

        public ISpecification<T> And(Expression<Func<T, bool>> predicate)
        {
            return new Specification<T>(this.Predicate.And(predicate));
        }

        public ISpecification<T> Or(ISpecification<T> specification)
        {
            return new Specification<T>(this.Predicate.Or(specification.Predicate));
        }

        public ISpecification<T> Or(Expression<Func<T, bool>> predicate)
        {
            return new Specification<T>(this.Predicate.Or(predicate));
        }

        public T SatisfyingItemFrom(IQueryable<T> query)
        {
            return query.Where(Predicate).SingleOrDefault();
        }

        public IQueryable<T> SatisfyingItemsFrom(IQueryable<T> query)
        {
            return query.Where(Predicate);
        }
    }
}
