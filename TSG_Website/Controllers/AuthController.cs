using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using TSG_Website.Data;
using TSG_Website.DTOs;
using TSG_Website.Models;
using TSG_Website.Services;


namespace TSG_Website.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IEmailService emailService, IConfiguration config)
        {
            _context = context;
            _emailService = emailService;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Email sau parolă incorectă." });
            }

            var token = GenerateToken(user);
            return Ok(new { token, user.Email });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/process")]
        public async Task<IActionResult> Process(int id, [FromBody] ProcessRegistrationDto dto)
        {
            var reg = await _context.Registrations.FindAsync(id);
            if (reg == null) 
                return NotFound();
            if (reg.Status != RegistrationStatus.Pending)
                return BadRequest(new { message = "Înregistrarea a fost deja procesată." });

            if (dto.Action == "accept")
            {
                var plainPassword = GeneratePassword();

                var user = new User
                {
                    Email = reg.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword),
                    FirstName = reg.FirstName,
                    LastName = reg.LastName,
                    PhoneNumber = reg.PhoneNumber,
                    IsActive = true,
                };

                _context.Users.Add(user);
                reg.Status = RegistrationStatus.Accepted;
                await _context.SaveChangesAsync();

                reg.UserId = user.Id;
                await _context.SaveChangesAsync();

                await _emailService.SendWelcomeEmailAsync(user.Email, (reg.FirstName + " " + reg.LastName), plainPassword);
                return Ok(new { message = "Înregistrarea a fost acceptată și un cont a fost creat." });
            }

            // Handle rejection case
            reg.Status = RegistrationStatus.Rejected;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Înregistrarea a fost respingă." });
        }

        private static string GeneratePassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789@#$!";
            var rng = new Random();
            var pwd = new char[12];
            for (int i = 0; i < 12; i++)
                pwd[i] = chars[rng.Next(chars.Length)];
            return new string(pwd);
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
