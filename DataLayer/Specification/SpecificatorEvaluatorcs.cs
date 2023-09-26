using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Specification
{
    public static class SpecificationEvaluator<TEntity> where TEntity : class
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery,
            ISpecification<TEntity> specification)
        {
            var query = inputQuery;

            if (specification.Expression != null)
            {
                query = query.Where(specification.Expression);
            }

            if (specification.IncludeExpressions.Count > 0)
            {
                query = specification.IncludeExpressions.Aggregate((query), (current, include) =>
                    current.Include(include));
            }

            if (specification.Paging != null)
            {
                query = query.Skip(specification.Paging.Skip)
                    .Take(specification.Paging.Take);
            }

            return query;
        }
    }
}
