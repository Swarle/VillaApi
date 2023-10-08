using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogicLayer.Dto
{
    public class VillaCreateDto
    {
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
        public double Rate { get; set; }
        [Required]
        public double Sqmt { get; set; }
        public int? Occupancy { get; set; }
        [Required]
        public decimal Price { get; set; }

        public bool IsNullOrEmpty()
        {
            return Name.IsNullOrEmpty() || Describe.IsNullOrEmpty() || ImageUrl.IsNullOrEmpty() ||
                   VillaNumber == default || Rate == default || Sqmt == default || Price == default;
        }
    }
}
