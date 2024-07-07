using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Timers;
using backend.Entity;
using backend.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using webapi.Dao.IServices;
using webapi.Data;
using webapi.Model;

namespace webapi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IJWTManagerService jWTManager;
        private readonly IUserService userService;
        private readonly SmtpClient _smtpClient;
        private readonly IConfiguration _configuration;
        private System.Timers.Timer _timer;
        private readonly IServiceProvider _serviceProvider;

        public UsersController(
            DataContext context,
            IJWTManagerService jWTManager,
            IUserService userService,
            IConfiguration configuration,
            IServiceProvider serviceProvider
        )
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _context = context;

            this.jWTManager = jWTManager;
            this.userService = userService;

            _smtpClient = new SmtpClient
            {
                Host = _configuration["SmtpConfig:SmtpServer"],
                Port = int.Parse(_configuration["SmtpConfig:Port"]),
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    _configuration["SmtpConfig:Username"],
                    _configuration["SmtpConfig:Password"]
                ),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetListUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return await _context.Users.ToListAsync();
        }

        // Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLogin user)
        {
            var validUser = await userService.IsValidUserAsync(user.Username, user.Password);

            if (validUser is null)
            {
                return Unauthorized(new { Message = "Incorrect username or password!" });
            }

            var token = jWTManager.GenerateToken(user.Username, validUser.RoleId);

            if (token is null)
            {
                return Unauthorized(new { Message = "Invalid Attempt!" });
            }

            // Save refresh token to database
            UserRefreshTokens rfToken = new UserRefreshTokens()
            {
                RefreshToken = token.RefreshToken,
                UserName = user.Username,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(24)
            };

            userService.AddUserRefreshTokens(rfToken);
            userService.SaveCommit();

            var userInfo = new User()
            {
                Id = validUser.Id,
                Username = validUser.Username,
                Name = validUser.Name,
                Email = validUser.Email,
                RoleId = validUser.RoleId,
                Phone = validUser.Phone,
            };

            return Ok(new { token, userInfo });
        }

        // Refresh token
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Refresh(Tokens token)
        {
            var principal = jWTManager.GetPrincipalFromExpiredToken(token.AccessToken);
            var username = principal.Identity?.Name;
            var role = principal.Claims.ToArray()[4].Value;
            var savedRefreshToken = userService.GetSavedRefreshTokens(username, token.RefreshToken);

            if (
                savedRefreshToken?.RefreshToken != token.RefreshToken
                || savedRefreshToken?.RefreshTokenExpiryTime <= DateTime.UtcNow
            )
            {
                return Unauthorized(new { Message = "Invalid attempt!" });
            }

            var newJwtToken = jWTManager.GenerateRefreshToken(username, int.Parse(role));

            if (newJwtToken == null)
            {
                return Unauthorized(new { Message = "Invalid attempt!" });
            }

            UserRefreshTokens rfToken = new UserRefreshTokens
            {
                RefreshToken = newJwtToken.RefreshToken,
                UserName = username,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(24)
            };

            userService.DeleteUserRefreshTokens(username, token.RefreshToken);
            userService.AddUserRefreshTokens(rfToken);
            userService.SaveCommit();

            return Ok(newJwtToken);
        }

        // Register
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            //bool existedEmail = await _context.Users.AnyAsync(u => u.Email.Equals(user.Email));
            bool existedUsername = await _context.Users.AnyAsync(
                u => u.Username.Equals(user.Username)
            );

            if (existedUsername)
            {
                ModelState.AddModelError("DuplicatedUsername", "Username is duplicated");
            }

            if (!ModelState.IsValid)
            {
                return Ok(ModelState);
            }

            User newUser = new User();

            if (user.RoleId == null)
            {
                return BadRequest(new { message = "RoleId is required" });
            }
            else
            {
                var IsRole = await _context.Roles.FirstOrDefaultAsync(r => r.Id == user.RoleId);
                if (IsRole is null)
                {
                    return NotFound(new { message = "Role is not found" });
                }
                else
                {
                    newUser.Email = user.Email;
                    newUser.Name = user.Name;
                    newUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    newUser.Username = user.Username;
                    newUser.RoleId = user.RoleId;
                    newUser.Phone = user.Phone;
                    newUser.IsActive = user.IsActive;
                    newUser.CreateBy = user?.CreateBy;
                    newUser.CreateDate = user.CreateDate;
                    newUser.UpdateDate = user?.UpdateDate;
                    newUser.UpdateBy = user?.UpdateBy;

                    if (!existedUsername)
                    {
                        _context.Users.Add(newUser);
                        _context.SaveChanges();
                    }
                }
            }
            return Ok(newUser);
        }

        // Log out
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Logout([FromBody] Tokens token)
        {
            System.Console.WriteLine(token);
            var rfToken = await _context.UserRefreshTokens.FirstOrDefaultAsync(
                t => t.RefreshToken.Equals(token.RefreshToken)
            );

            if (rfToken is null)
            {
                return NotFound(new { message = "Invalid refresh token" });
            }

            _context.UserRefreshTokens.Remove(rfToken);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Log out successfully" });
        }

        // Forgot password
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SendForgotPasswordEmail(
            [FromBody] ForgotPasswordRequest request
        )
        {
            var isEmailValid = await _context.Users.FirstOrDefaultAsync(
                u => u.Email == request.Email
            );

            if (isEmailValid is null)
            {
                return NotFound(new { message = "User is not existed" });
            }

            var confirmationCode = GenerateRandomCode();

            request.ExpirationTime = DateTime.UtcNow.AddMinutes(2);
            request.Code = confirmationCode;

            await _context.ForgotPasswordRequests.AddAsync(request);
            await _context.SaveChangesAsync();

            SendConfirmationEmail(request.Email, confirmationCode);

            return Ok(new { message = "Xác nhận đã được gửi đến địa chỉ email của bạn." });
        }

        // Verify code reset password
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCodeResetPassword(
            [FromBody] ForgotPasswordRequest request
        )
        {
            var confirmCode = await _context.ForgotPasswordRequests
                .Where(f => f.Email == request.Email && f.Code == request.Code)
                .FirstOrDefaultAsync();

            if (confirmCode is null)
            {
                return NotFound(new { message = "confirm code invalid" });
            }

            if (confirmCode.ExpirationTime < DateTime.UtcNow)
            {
                _context.ForgotPasswordRequests.Remove(confirmCode);
                await _context.SaveChangesAsync();
                return NotFound(new { message = "The code has expired" });
            }

            _context.ForgotPasswordRequests.Remove(confirmCode);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Comfirm successfully" });
        }

        // Delete confirm code
        [HttpDelete]
        [AllowAnonymous]
        public async Task<ActionResult> DeleteConfirmCodeExpired()
        {
            DateTime currentTime = DateTime.UtcNow;

            var ConfirmCodeExpired = _context.ForgotPasswordRequests
                .Where(f => f.ExpirationTime < currentTime)
                .ToList();

            if (ConfirmCodeExpired.Count == 0)
            {
                return NotFound(new { message = "Code is not found" });
            }

            foreach (var confirmCode in ConfirmCodeExpired)
            {
                _context.ForgotPasswordRequests.Remove(confirmCode);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Delete successfully" });
        }

        private string GenerateRandomCode()
        {
            Random random = new Random();
            const string chars = "0123456789";
            return new string(
                Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray()
            );
        }

        // Send email comfirm code
        private void SendConfirmationEmail(string email, string confirmationCode)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["SmtpConfig:Username"]),
                Subject = "Xác nhận quên mật khẩu",
                Body = $"Mã xác nhận của bạn là: {confirmationCode}",
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            _smtpClient.Send(mailMessage);
        }
    }
}
