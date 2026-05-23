using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            // Trimite mailul de confirmare a primirii înscrierii
            await _emailService.SendRegistrationConfirmationAsync(registration.Email, registration.FirstName + " " + registration.LastName);

            return Ok(registration);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllRegistrations()
        {
            var registrations = await _context.Registrations
                .OrderByDescending(r => r.SubmittedAt)
                .ToListAsync();
            return Ok(registrations);
        }
    }
}
