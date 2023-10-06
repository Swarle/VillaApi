using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dto
{
    public class VillaDto
    {
        public Guid Id {get; set; }
        public string Describe { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public int VillaNumber { get; set; }
        public Guid VillaDetailsId { get; set; }
        public double Rate { get; set; }
        public double Sqmt { get; set; }
        public int? Occupancy { get; set; }
        public Guid VillaStatusId { get; set; }
        public string VillaStatus { get; set; } = null!;
    }
}
