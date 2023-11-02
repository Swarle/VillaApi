using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dto.Order
{
    public class OrderStatusDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = null!;
    }
}
