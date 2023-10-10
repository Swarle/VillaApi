using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.UserSpecification
{
    public class FindUserByLoginSpecification : BaseSpecification<Users>
    {
        public FindUserByLoginSpecification(string login,bool includeRole = false) : base(u => u.Login == login)
        {
            if(includeRole)
                AddInclude(e => e.Role);

            AsNoTracking = true;
        }


    }
}
