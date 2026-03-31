using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TSG_Website.Models
{
    public enum RegistrationStatus { Pending, Accepted, Rejected }

    public class Registration
    {
        [Key]
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string CVUrl { get; set; } = string.Empty;
        public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        
        [ForeignKey("User")]
        public Guid? UserId { get; set; }
        public User? User { get; set; }
    }
}
