using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Dto;

namespace UnitTests.BusinessTests.EqualityComparers
{
    public class VillaPartialDtoEqualityComparer : IEqualityComparer<VillaPartialDto>
    {
        public bool Equals(VillaPartialDto? x, VillaPartialDto? y)
        { 
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Id == y.Id && x.Describe == y.Describe && x.Name == y.Name && x.ImageUrl == y.ImageUrl &&
                   x.VillaNumber == y.VillaNumber && x.Price == y.Price;
        }

        public int GetHashCode(VillaPartialDto obj)
        {
            return HashCode.Combine(obj.Id, obj.Name, obj.Describe, obj.ImageUrl, obj.VillaNumber, obj.Price);
        }
    }
}
