using BusinessLogicLayer.Dto.User;
using BusinessLogicLayer.Dto.Villa;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dto.Order
{
    public class OrderCreateDto
    {
        [Required]
        public DateTime CheckIn { get; set; }
        [Required]
        public DateTime CheckOut { get; set; }
        [JsonIgnore]
        public Guid StatusId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid VillaId { get; set; }
    }
}
