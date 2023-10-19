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
        public double Rate { get; set; }
        [Required]
        public double Sqmt { get; set; }
        public int? Occupancy { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        [MaxLength(20)]
        public string Status { get; set; }
    }
}
