using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Dto.User;
using BusinessLogicLayer.Dto.Villa;

namespace BusinessLogicLayer.Dto.Order
{
    public class OrderPartialDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public UserPartialDto User { get; set; }
        public VillaPartialDto Villa { get; set; }
        public string Status { get; set; } = null!;
    }
}
