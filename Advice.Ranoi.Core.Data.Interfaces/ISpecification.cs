using Advice.Ranoi.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Advice.Ranoi.Core.Data.Interfaces
{
    public interface ISpecification<T> where T : IEntity
    {
        Expression<Func<T, bool>> Predicate { get; }

        ISpecification<T> And(ISpecification<T> specification);

        ISpecification<T> And(Expression<Func<T, bool>> predicate);

        ISpecification<T> Or(ISpecification<T> specification);

        ISpecification<T> Or(Expression<Func<T, bool>> predicate);

        T SatisfyingItemFrom(IQueryable<T> query);

        IQueryable<T> SatisfyingItemsFrom(IQueryable<T> query);

    }
}
