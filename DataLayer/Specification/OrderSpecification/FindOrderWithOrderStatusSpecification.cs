using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.OrderSpecification
{
    public class FindOrderWithOrderStatusSpecification : BaseSpecification<Orders>
    {
        public FindOrderWithOrderStatusSpecification(Guid id) : base(o => o.Id == id)
        {
            AddInclude(o => o.Status);
        }
    }
}
