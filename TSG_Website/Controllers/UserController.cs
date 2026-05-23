using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TSG_Website.Data;
using TSG_Website.DTOs;

namespace TSG_Website.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("me")]
        public async Task<IActionResult> GetUserProfile()
        {
            var user = await _db.Users.FindAsync(GetUserId());
            if (user == null) return NotFound();

            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Bio,
                user.LinkedInUrl,
                user.PhoneNumber,
            });
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateProfileDto dto)
        {
            var user = await _db.Users.FindAsync(GetUserId());
            if (user == null) return NotFound();

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.Bio = dto.Bio ?? user.Bio;
            user.LinkedInUrl = dto.LinkedInUrl ?? user.LinkedInUrl;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;

            await _db.SaveChangesAsync();
            return Ok(new { message = "Profil actualizat." });
        }

        [HttpPut("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var user = await _db.Users.FindAsync(GetUserId());
            if (user == null) return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                return BadRequest(new { message = "Parola curent? este incorect?." });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Parola schimbat?." });
        }

        [HttpDelete("me")]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _db.Users.FindAsync(GetUserId());
            if (user == null) return NotFound();

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Contul a fost șters cu succes." });
        }

        [HttpDelete("{id}/delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserAccount(Guid id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Contul de utilizator a fost șters cu succes." });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _db.Users
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Bio,
                    u.LinkedInUrl,
                    u.PhoneNumber,
                    u.IsAdmin,
                    u.IsActive
                })
                .ToListAsync();
            return Ok(users);
        }
    }
}