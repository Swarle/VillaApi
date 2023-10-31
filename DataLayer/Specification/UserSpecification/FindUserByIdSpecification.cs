using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.UserSpecification
{
    public class FindUserByIdSpecification : BaseSpecification<Users>
    {
        public FindUserByIdSpecification(Guid id) : base(u => u.Id == id) { }
    }
}
