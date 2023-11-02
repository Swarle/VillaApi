using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dto.Order
{
    public class OrderChangeStatusDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid StatusId { get; set; }
    }
}
