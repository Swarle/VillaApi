using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.RoleSpecification
{
    public class FindRoleSpecification : BaseSpecification<Role>
    {
        public FindRoleSpecification(string roleName) : base(r => r.RoleName == roleName) { }
    }
}
