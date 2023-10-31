using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Dto.User;
using BusinessLogicLayer.Dto.Villa;

namespace BusinessLogicLayer.Dto.Order
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string Status { get; set; } = null!;
        public UserPartialDto User { get; set; }
        public VillaDto Villa { get; set; }

    }
}
