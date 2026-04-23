using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TSG_Website.Data;
using TSG_Website.Models;

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
            var posts = await _context.BlogPosts.ToListAsync();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticle(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> CreateArticle([FromBody] BlogPost post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            post.CreatedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;

            _context.BlogPosts.Add(post);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetArticle), new { id = post.Id }, post);
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
