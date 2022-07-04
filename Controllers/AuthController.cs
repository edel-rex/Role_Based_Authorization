using jwt_employee.Constants;
using jwt_employee.Data;
using jwt_employee.Interface;
using jwt_employee.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Web.HttpUtility;

namespace jwt_employee.Controllers
{
    [Route(DbConstants.Route_API)]
    [ApiController]
    public class AuthController : ControllerBase
    {

        public IConfiguration _configuration;
        public readonly AppDbContext _context;
        private readonly IUserInfos _userInfo;

        private UserInfo _user = new();
        private Email _emailContent = new();
        private readonly IEmailService _emailService;

        public AuthController(IConfiguration config, IUserInfos userinfo, AppDbContext context, IEmailService emailService)
        {
            _configuration = config;
            _context = context;
            _userInfo = userinfo;
            _emailService = emailService;
        }


        [HttpPost(DbConstants.Route_Register)]
        public async Task<ActionResult<string>> Register(UserRegistration request)
        {

            if (_context.UserInfos!.Where(u => u.UserName == request.UserName).IsNullOrEmpty())
            {

                CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                _user.UserName = request.UserName;
                _user.DisplayName = request.UserName;
                _user.Email = request.Email;
                _user.PasswordSalt = passwordSalt;
                _user.PasswordHash = passwordHash;
                _user.CreatedDate = DateTime.Now;
                _user.Auth_Role = request.Role;
                _user.EmailValidationToken = GenerateRandomToken();



                var refreshToken = GenerateRefreshToken();

                _user.RefreshToken = refreshToken.ToString()!;

                setRefreshToken(refreshToken);
                _userInfo.AddUser(_user);

                string token = CreateToken(_user);


                _emailContent.To = request.Email;
                _emailContent.Subject = "Verify your Email";
                _emailContent.Body = $@"
                <html>
                      <body>
                      <p>Dear {request.UserName},</p>
                      <p>Please Verify your email by clicking the below link.</p>
                      <br>
                      <a href='https://localhost:7156/api/verify?token={UrlEncode(_user.EmailValidationToken)}'>Click here to Verify</a>
                      <p>Sincerely,<br>-Kovai.co</br></p>
                      </body>
                      </html>
                ";

                _emailService.SendEmail(_emailContent);

                return Ok(token);
            }
            else
            {
                UserLogin login_user = new();
                login_user.Email = request.Email;
                login_user.Password = request.Password;

                var result = await Login(login_user);
                return result;

            }


        }


        [HttpPost(DbConstants.Route_Login)]
        public async Task<ActionResult<string>> Login(UserLogin request)
        {
            if (request != null && request.Email != null && request.Password != null)
            {
                _user = await GetUser(email: request.Email);

                if (_user != null)
                {

                    byte[]? passwordHash = _context.UserInfos!.Single(u => u.Email == request.Email).PasswordHash;
                    byte[]? passwordSalt = _context.UserInfos!.Single(u => u.Email == request.Email).PasswordSalt;

                    if (!VerifyPasswordHash(request.Password, passwordHash, passwordSalt)) return BadRequest("Wrong Credentials");
                    //create claims details based on the user information

                    var refreshToken = GenerateRefreshToken();
                    setRefreshToken(refreshToken);
                    _userInfo.UpdateUser(_user);

                    string token = CreateToken(_user);
                    return Ok(token);

                }
                else
                {
                    return BadRequest("Invalid Credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }


        [HttpGet(DbConstants.Route_Verify)]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var find_user = await GetUser(token: token);
            if (find_user == null) return BadRequest("Invalid Token");

            find_user.VerifiedAt = DateTime.Now;
            _userInfo.UpdateUser(find_user);

            return Ok("User Verified");
        }

        [HttpPost(DbConstants.Route_Forget_Password)]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            _user = await GetUser(email: email);
            if (_user == null) return BadRequest("User not found");

            _user.ResetToken = GenerateRandomToken();
            _user.ResetTokenExpires = DateTime.Now.AddDays(1);
            _userInfo.UpdateUser(_user);

            _emailContent.To = email;
            _emailContent.Subject = "Reset your Password";
    //         _emailContent.Body = @$"
    //         <html>
    //             <body>
    //                         <legend> Confirm password with HTML5 </legend>

    //                         <input type='password' placeholder='Password' id='password' required>
    //                         <input type='password' placeholder='Confirm Password' id='confirm_password' required>

    //                         <button class='pure-button pure-button-primary'>Confirm</button>

    //             </body>
    //              <script type='module' src='https://raw.githubusercontent.com/edel-rex/Kovai/master/ResetPassword.js' async defer></script>
    //         </html>
    // ";

            _emailContent.Body = _user.ResetToken;

            _emailService.SendEmail(_emailContent);

            return Ok("Check your mail");
        }

        [HttpPost(DbConstants.Route_Reset_Password)]
        public async Task<IActionResult> ResetPassword(UserResetPassword request)
        {
            _user = await GetUser(token: request.token);
            if (_user == null) return BadRequest("User not found");
            if (_user.ResetTokenExpires < DateTime.Now) return BadRequest("Password already reseted");

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            _user.PasswordHash = passwordHash;
            _user.PasswordSalt = passwordSalt;
            _user.ResetTokenExpires = DateTime.Now;
            var refreshToken = GenerateRefreshToken();
            _user.RefreshToken = refreshToken.ToString()!;
            setRefreshToken(refreshToken);
            _userInfo.UpdateUser(_user);

            string token = CreateToken(_user);

            return Ok(token);
        }

        [HttpPost(DbConstants.Route_Refresh_Token)]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var user_id = Convert.ToInt32(HttpContext.Items[DbConstants.Claims_UserID]);
            _user = await GetUser(user_id: user_id);

            var refreshToken = Request.Cookies[DbConstants.RefreshToken];
            if (!_user.RefreshToken.Equals(refreshToken)) return Unauthorized("Invalid Refresh Token");
            else if (_user.TokenExpires < DateTime.Now) return Unauthorized("Token expired");


            var _refreshToken = GenerateRefreshToken();
            setRefreshToken(_refreshToken);
            _userInfo.UpdateUser(_user);

            string token = CreateToken(_user);
            return Ok(token);
        }

        [NonAction]
        private string CreateToken(UserInfo user)
        {
            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration[DbConstants.ValidSubject]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim(DbConstants.Claims_UserID, user.UserID.ToString()),
                        new Claim(DbConstants.Claims_DisplayName, user.DisplayName),
                        new Claim(DbConstants.Claims_UserName, user.UserName),
                        new Claim(DbConstants.Claims_Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Auth_Role)
                    };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[DbConstants.IssuerSigningKey]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration[DbConstants.ValidIssuer],
                _configuration[DbConstants.ValidAudience],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signIn
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [NonAction]
        private async Task<UserInfo> GetUser(int user_id = 0, string email = "", string token = "")
        {
            var result = await _context.UserInfos!.FirstOrDefaultAsync(u => u.UserID == user_id || u.Email == email || u.EmailValidationToken == token || u.ResetToken == token);
            return result!;
        }


        [NonAction]
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }

        [NonAction]
        private bool VerifyPasswordHash(string password, byte[] PasswordHash, byte[] PasswordSalt)
        {
            using (var hmac = new HMACSHA512(PasswordSalt))
            {
                var ComputedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                Console.WriteLine(ComputedHash.ToString());
                return ComputedHash.SequenceEqual(PasswordHash);
            }
        }

        [NonAction]
        private void setRefreshToken(RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append(DbConstants.RefreshToken, newRefreshToken.Token, cookieOptions);

            _user.RefreshToken = newRefreshToken.Token;
            _user.TokenCreated = newRefreshToken.Created;
            _user.TokenExpires = newRefreshToken.Expires;


        }

        [NonAction]
        private RefreshToken GenerateRefreshToken()
        {
            RefreshToken refreshToken = new()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now,
            };

            return refreshToken;
        }

        [NonAction]
        private string GenerateRandomToken()
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            if (_user.EmailValidationToken == token)
            {
                GenerateRandomToken();
            }
            return token;
        }
    }
}
