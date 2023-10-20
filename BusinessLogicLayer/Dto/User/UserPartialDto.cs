using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Dto.Order;

namespace BusinessLogicLayer.Dto.User
{
    public class UserPartialDto
    { 
        public Guid Id { get; set; }

        public string Login { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;
        
        public string Role { get; set; } = null!;
        
        public int OrdersCount { get; set; }

    }
}
