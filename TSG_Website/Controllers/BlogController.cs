using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TSG_Website.Data;
using TSG_Website.Models;
using TSG_Website.DTOs;

namespace TSG_Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetArticles()
        {
            var posts = await _context.BlogPosts
                .Include(p => p.Author)          // include datele autorului
                .Where(p => p.Status == "published")
                .OrderByDescending(p => p.PublishedAt)
                .Select(p => new {
                    p.Id,
                    p.Title,
                    p.Content,
                    p.ImageUrl,
                    p.Status,
                    p.CreatedAt,
                    p.PublishedAt,
                    p.ViewCount,
                    Author = new
                    {
                        p.Author.FirstName,
                        p.Author.LastName
                    }
                })
                .ToListAsync();
            return Ok(posts);
        }

        // GET /api/blog/all — admin, toate articolele
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _context.BlogPosts
                .Include(p => p.Author)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new {
                    p.Id,
                    p.Title,
                    p.Status,
                    p.CreatedAt,
                    p.PublishedAt,
                    p.ViewCount,
                    Author = new { p.Author.FirstName, p.Author.LastName }
                })
                .ToListAsync();

            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticle(int id)
        {
            var post = await _context.BlogPosts
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            post.ViewCount++;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                post.Id,
                post.Title,
                post.Content,
                post.ImageUrl,
                post.Status,
                post.CreatedAt,
                post.PublishedAt,
                post.ViewCount,
                Author = new { post.Author.FirstName, post.Author.LastName }
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateArticle([FromBody] CreateBlogPostDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var role = User.FindFirstValue(ClaimTypes.Role);

            var post = new BlogPost
            {
                Title = dto.Title,
                Content = dto.Content,
                ImageUrl = dto.ImageUrl,
                AuthorId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                // adminul publică direct, userul trimite spre aprobare
                Status = role == "Admin" ? "published" : "pending",
                PublishedAt = role == "Admin" ? DateTime.UtcNow : null
            };

            _context.BlogPosts.Add(post);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = role == "Admin"
                    ? "Articol publicat."
                    : "Articol trimis spre aprobare.",
                post.Id
            });
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null) return NotFound();

            post.Status = "published";
            post.PublishedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Articol aprobat și publicat." });
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null) return NotFound();

            post.Status = "rejected";
            post.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Articol respins." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, [FromBody] BlogPost updatedPost)
        {
            if (id != updatedPost.Id)
            {
                return BadRequest();
            }

            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            post.Title = updatedPost.Title;
            post.Content = updatedPost.Content;
            post.ImageUrl = updatedPost.ImageUrl;
            post.Status = updatedPost.Status;
            post.PublishedAt = updatedPost.PublishedAt;
            post.UpdatedAt = DateTime.UtcNow;
            _context.BlogPosts.Update(post);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.BlogPosts.Remove(post);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
