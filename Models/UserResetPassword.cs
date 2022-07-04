using System;
using System.ComponentModel.DataAnnotations;

namespace jwt_employee.Models
{
    public class UserResetPassword
    {
        [Required]
        public string token { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
