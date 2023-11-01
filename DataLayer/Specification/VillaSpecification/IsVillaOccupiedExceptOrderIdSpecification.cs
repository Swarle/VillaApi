using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.VillaSpecification
{
    public class IsVillaOccupiedExceptOrderIdSpecification : BaseSpecification<Villa>
    {
        public IsVillaOccupiedExceptOrderIdSpecification(Guid id,Guid orderId, DateTime checkIn, DateTime checkOut) :
            base(v => v.Id == id && v.Orders.Any(o => o.Id != orderId &&
                (checkIn >= o.CheckIn && checkIn <= o.CheckOut) || (checkOut >= o.CheckIn && checkOut <= o.CheckOut) || (checkIn <= o.CheckIn && checkOut >= o.CheckOut)))
        {
            AddInclude(v => v.Orders);
        }
    }
}
