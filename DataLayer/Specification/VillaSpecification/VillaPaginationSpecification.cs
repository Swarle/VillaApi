using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;
using DataLayer.Specification.Infrastructure;

namespace DataLayer.Specification.VillaSpecification
{
    public class VillaPaginationSpecification : BaseSpecification<Villa>
    {
        public VillaPaginationSpecification(PagingSpecification pagingSpecification) : base()
        {
            AddPagination(pagingSpecification);
        }
    }
}
