using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.Dto.User;

namespace BusinessLogicLayer.Dto
{
    public class AuthResponseDto
    {
        public AuthUserDto User { get; set; }
        public string Token { get; set; }
    }
}
