using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Dto.Order;

namespace BusinessLogicLayer.Dto.User
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string Login { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Role { get; set; } = null!;

        public OrderPartialDto Orders { get; set; }
    }
}
