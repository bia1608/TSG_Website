using System.ComponentModel.DataAnnotations;

namespace TSG_Website.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsAdmin { get; set; }

        public List<BlogPost> Posts { get; set; } = new();

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        public string Bio { get; set; } = "Membru TSG";
        public string? LinkedInUrl { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
