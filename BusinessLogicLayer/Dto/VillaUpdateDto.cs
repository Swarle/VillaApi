using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dto
{
    public class VillaUpdateDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
        [Required]
        [MaxLength(200)]
        public string Describe { get; set; } = null!;
        [Required]
        public string ImageUrl { get; set; } = null!;
        [Required]
        public int VillaNumber { get; set; }
        [Required]
        public Guid VillaDetailsId { get; set; }
        [Required]
        public double Rate { get; set; }
        [Required]
        public double Sqmt { get; set; }
        public int? Occupancy { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        [MaxLength(20)]
        public string Status { get; set; }

        public bool IsNullOrEmpty()
        {
            return Id == Guid.Empty || VillaDetailsId == Guid.Empty ||
                Name.IsNullOrEmpty() || Describe.IsNullOrEmpty() || ImageUrl.IsNullOrEmpty() || Status.IsNullOrEmpty() ||
                   VillaNumber <= 0 || Rate < 0 || Sqmt <= 0 || Price <= 0;
        }
    }
}
