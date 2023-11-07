using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogicLayer.Dto.Villa
{
    public class VillaCreateDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Describe { get; set; } = null!;

        internal string? ImageUrl { get; set; } = null!;
        
        public IFormFile? Image { get; set; } = null!;

        internal string? ImageLocalPath { get; set; }

        [Required]
        public int VillaNumber { get; set; }

        [Required]
        public double Rate { get; set; }

        [Required]
        public double Sqmt { get; set; }

        public int? Occupancy { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
