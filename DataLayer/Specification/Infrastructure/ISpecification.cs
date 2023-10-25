using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Specification.Infrastructure
{
    public interface ISpecification<TEntity> where TEntity : class
    {
        public Expression<Func<TEntity, bool>> Expression { get; }
        public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; set; }
        public List<string> IncludeString { get; set; }
        public PagingSpecification Paging { get; set; }
        public bool AsNoTracking { get; set; }

        bool IsSatisfied(TEntity obj);
    }
}
