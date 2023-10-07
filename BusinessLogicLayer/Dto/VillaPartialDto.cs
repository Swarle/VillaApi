using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dto
{
    public class VillaPartialDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Describe { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public int VillaNumber { get; set; }
        public decimal Price { get; set; }

    }
}
