using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Specification.OrderSpecification
{
    public class FindOrderWithContactsAndVillasSpecification : BaseSpecification<Orders>
    {
        public FindOrderWithContactsAndVillasSpecification()
        {
            Includes();
        }
        public FindOrderWithContactsAndVillasSpecification(Guid id) : base(o => o.Id == id)
        {
            Includes();
        }

        void Includes()
        {
            AddInclude(e => e.Villa);
            AddInclude($"{nameof(Villa)}.{nameof(Villa.VillaDetails)}");
            AddInclude($"{nameof(Villa)}.{nameof(Villa.Status)}");
            AddInclude(e => e.User);
            AddInclude(e => e.User.Role);
            AddInclude(e => e.Status);
        }
    }
}
