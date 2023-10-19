using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Dto.Villa;

namespace UnitTests.BusinessTests.EqualityComparers
{
    internal class VillaDtoEqualityComparer : IEqualityComparer<VillaDto>
    {
        public bool Equals(VillaDto? x, VillaDto? y)
        {
            if(ReferenceEquals(x,y)) return true;
            if(ReferenceEquals(x,null)) return false;
            if(ReferenceEquals(y, null)) return false;
            if(x.GetType() != y.GetType()) return false;
            return x.Id == y.Id && x.Name == y.Name && x.Describe == y.Describe && x.ImageUrl == y.ImageUrl && x.VillaNumber == y.VillaNumber && 
                    x.Rate == y.Rate && x.Sqmt == y.Sqmt && x.Occupancy == y.Occupancy &&
                   x.VillaStatus == y.VillaStatus && x.Price == y.Price;
        }

        public int GetHashCode(VillaDto obj)
        {
            return HashCode.Combine(obj.Id,obj.Name,obj.Price,obj.Describe,obj.ImageUrl);
        }
    }
}
