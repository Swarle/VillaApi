using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.UserSpecification
{
    public class FindUserWithRoleSpecification : BaseSpecification<Users>
    {
        public FindUserWithRoleSpecification()
        {
            AddInclude(u => u.Role);
        }

        public FindUserWithRoleSpecification(Guid id) : base(u => u.Id == id)
        {
            AddInclude(u => u.Role);
        }
    }
}
