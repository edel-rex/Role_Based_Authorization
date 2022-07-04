using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace jwt_employee.Models
{
    public class UserInfo
    {
        [Key]
        public int UserID { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Auth_Role { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
        public string EmailValidationToken { get; set; } = string.Empty;
        public string ResetToken { get; set; } = string.Empty;
        public DateTime ResetTokenExpires { get; set; }
        public DateTime VerifiedAt { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
