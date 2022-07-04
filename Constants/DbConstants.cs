using System;

namespace jwt_employee.Constants
{
    public static class DbConstants
    {
        public const string DbContext = "AppDbContext";

        // Role
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        // Route
        public const string Route_Login = "login";
        public const string Route_Register = "Register";
        public const string Route_Employee = "api/employee";
        public const string Route_API = "api";
        public const string Route_Refresh_Token = "refresh_token";
        public const string Route_Verify = "verify";
        public const string Route_Forget_Password = "forget_password";
        public const string Route_Reset_Password = "reset_password";

        // JWT
        public const string ValidAudience = "Jwt:Audience";
        public const string ValidIssuer = "Jwt:Issuer";
        public const string IssuerSigningKey = "Jwt:Key";
        public const string ValidSubject = "Jwt:Subject";
        public const string Bearer = "Bearer";
        public const string Authorization = "Authorization";
        public const string JWT = "JWT";
        public const string RefreshToken = "refreshToken";

        // Claims
        public const string Claims_UserID = "UserID";
        public const string Claims_DisplayName = "DisplayName";
        public const string Claims_UserName = "UserName";
        public const string Claims_Email = "Email";
    }
}
