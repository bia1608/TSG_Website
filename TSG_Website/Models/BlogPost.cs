using System.ComponentModel.DataAnnotations;

namespace TSG_Website.Models { 

public class BlogPost
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Status { get; set; } = "pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAt { get; set; }
    public int ViewCount { get; set; } = 0;

    public Guid AuthorId { get; set; }
    public User Author { get; set; } = null!;
}
}