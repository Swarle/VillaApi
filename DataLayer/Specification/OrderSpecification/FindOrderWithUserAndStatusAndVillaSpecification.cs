using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.OrderSpecification
{
    public class FindOrderWithUserAndStatusAndVillaSpecification : BaseSpecification<Orders>
    {
        public FindOrderWithUserAndStatusAndVillaSpecification()
        {
            AddInclude(e => e.Villa);
            AddInclude($"{nameof(Villa)}.{nameof(Villa.Status)}");
            AddInclude(e => e.User);
            AddInclude(e => e.User.Role);
            AddInclude(e => e.Status);
        }
    }
}
