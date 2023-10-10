using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.RoleSpecification
{
    public class IsRoleExistSpecification : BaseSpecification<Role>
    {
        public IsRoleExistSpecification(string roleName) : base(r => r.RoleName == roleName) { }
    }
}
