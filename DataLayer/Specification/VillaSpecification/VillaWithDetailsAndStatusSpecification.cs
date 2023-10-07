using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.VillaSpecification
{
    public sealed class VillaWithDetailsAndStatusSpecification : BaseSpecification<Villa>
    {
        public VillaWithDetailsAndStatusSpecification()
        {
            Includes();
        }

        public VillaWithDetailsAndStatusSpecification(Guid id) : base(x => x.Id == id)
        {
            Includes();
        }

        public override bool IsSatisfied(Villa obj)
        {
            return obj.VillaDetails != null && obj.Status != null && obj.VillaDetailsId != Guid.Empty && obj.StatusId != Guid.Empty;
        }

        private void Includes()
        {
            AddInclude(x => x.VillaDetails);
            AddInclude(x => x.Status);
        }
    }
}
