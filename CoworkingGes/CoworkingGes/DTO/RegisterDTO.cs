using CoworkingGes.Enum;
using System.ComponentModel.DataAnnotations;

namespace CoworkingGes.DTO
{
    public class RegisterDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        [EmailAddress]

        public string EmailAddress { get; set; }

        public UserRole Role { get; set; }

    }
}
