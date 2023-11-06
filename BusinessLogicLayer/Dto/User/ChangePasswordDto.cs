using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dto.User
{
    public class ChangePasswordDto
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string NewPassword { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        [Compare("Password",ErrorMessage = "Passwords must match")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
