using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.VillaSpecification
{
    public sealed class FindVillaWithDetailsAndStatusSpecification : BaseSpecification<Villa>
    {
        public FindVillaWithDetailsAndStatusSpecification()
        {
            Includes();
        }

        public FindVillaWithDetailsAndStatusSpecification(Guid id, bool asNoTracking = false) : base(x => x.Id == id)
        {
            Includes();
            AsNoTracking = asNoTracking;
        }

        public override bool IsSatisfied(Villa obj)
        {
            return obj.VillaDetails != null && obj.Status != null && obj.StatusId != Guid.Empty;
        }

        private void Includes()
        {
            AddInclude(x => x.VillaDetails);
            AddInclude(x => x.Status);
        }
    }
}
