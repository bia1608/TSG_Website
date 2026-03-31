using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TSG_Website.Data;
using TSG_Website.DTOs;
using TSG_Website.Models;
using TSG_Website.Services;

namespace TSG_Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public RegisterController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitRegistration([FromBody] RegisterRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var registration = new Registration
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                CVUrl = dto.CVUrl,
                Status = RegistrationStatus.Pending,
                SubmittedAt = DateTime.UtcNow
            };

            _context.Registrations.Add(registration);
            await _context.SaveChangesAsync();
            return Ok(registration);
        }

        [HttpPost("{id}/process")]
        public async Task<IActionResult> AcceptUser(int id)
        {
            var registration = await _context.Registrations.FindAsync(id);
            if (registration == null)
                return NotFound();

            string password = GenerateRandomPassword();
            string hash = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Email = registration.Email,
                PasswordHash = hash,
                IsActive = true,
                IsAdmin = false,
                FirstName = registration.FirstName,
                LastName = registration.LastName,
                PhoneNumber = registration.PhoneNumber
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _emailService.SendWelcomeEmailAsync(user.Email, registration.FirstName, password);
            return Ok(new { message = "User created successfully" });
        }

        private static string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789@#$!";
            var rng = new Random();
            var pwd = new char[12];
            for (int i = 0; i < 12; i++)
                pwd[i] = chars[rng.Next(chars.Length)];
            return new string(pwd);
        }
    }
}
