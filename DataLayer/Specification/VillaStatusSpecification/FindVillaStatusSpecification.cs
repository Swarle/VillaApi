using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.VillaStatusSpecification
{
    public class FindVillaStatusSpecification : BaseSpecification<VillaStatus>
    {
        public FindVillaStatusSpecification(string status) : base(s => s.Status == status) { }

        public FindVillaStatusSpecification(Guid id) : base(s => s.Id == id){}
    }
}
