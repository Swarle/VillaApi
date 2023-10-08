using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.VillaStatusSpecification
{
    public class GetVillaStatusByName : BaseSpecification<VillaStatus>
    {
        public GetVillaStatusByName(string status) : base(s => s.Status == status) { }
    }
}
