using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.VillaSpecification
{
    public class IfVillaExist : BaseSpecification<Villa>
    {
        public IfVillaExist(string name, int villaNumber) : base(v => v.Name == name | v.VillaNumber == villaNumber)
        {

        }
    }
}
