using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.VillaSpecification
{
    public class IsVillaExist : BaseSpecification<Villa>
    {
        public IsVillaExist( string name, int villaNumber) : base(v => v.Name == name || v.VillaNumber == villaNumber)
        {

        }
        public IsVillaExist(Guid id, bool asNoTracking = false) : base(v => v.Id == id)
        {
            AsNoTracking = asNoTracking;
        }
    }
}
