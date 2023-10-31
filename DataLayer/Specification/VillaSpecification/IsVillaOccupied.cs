using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.VillaSpecification
{
    public class IsVillaOccupied : BaseSpecification<Villa>
    {
        public IsVillaOccupied(Guid id,DateTime checkIn,DateTime checkOut) :
            base(v => v.Id == id && v.Orders.Any(o => 
                (checkIn >= o.CheckIn && checkIn <= o.CheckOut) || (checkOut >= o.CheckIn && checkOut <= o.CheckOut) || (checkIn <= o.CheckIn && checkOut >= o.CheckOut)))
        {
            AddInclude(v => v.Orders);
        }
    }
}
