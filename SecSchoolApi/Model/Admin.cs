using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SecSchoolApi.Model
{
    public class Admin
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = default!;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = default!;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
    }
}
