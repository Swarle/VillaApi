using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.UserSpecification
{
    public class FindUserWithRoleAndOrdersSpecification : BaseSpecification<Users>
    {
        public FindUserWithRoleAndOrdersSpecification(Guid id) : base(entity => entity.Id == id)
        {
            AddInclude(user => user.Role);
            AddInclude(user => user.Orders);
            AddInclude($"{nameof(Orders)}.{nameof(Orders.Status)}");
            AddInclude($"{nameof(Orders)}.{nameof(Orders.Villa)}");
        }
    }
}
