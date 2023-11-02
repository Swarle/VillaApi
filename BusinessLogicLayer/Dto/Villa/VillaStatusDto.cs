using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dto.Villa
{
    public class VillaStatusDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = null!;
    }
}
