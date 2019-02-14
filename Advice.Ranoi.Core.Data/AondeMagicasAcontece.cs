using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Advice.Ranoi.Core.Data
{
    public class ParameterRebinder : ExpressionVisitor
    {

        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {

            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();

        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {

            return new ParameterRebinder(map).Visit(exp);

        }

        protected override Expression VisitParameter(ParameterExpression p)
        {

            ParameterExpression replacement;

            if (map.TryGetValue(p, out replacement))
            {

                p = replacement;

            }

            return base.VisitParameter(p);

        }
    }

    public static class Utility
    {
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // cria mapa de parâmetros (de parâmetros de segundo para parâmetros de primeiro)

            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);


            // substitui parâmetros na segunda expressão lambda por parâmetros do primeiro

            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // aplica composição de corpos de expressão lambda a parâmetros da primeira expressão

            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);

        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {

            return first.Compose(second, Expression.And);

        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {

            return first.Compose(second, Expression.Or);

        }

    }
}
