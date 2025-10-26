using System.ComponentModel.DataAnnotations;

namespace SecSchoolApi.Model
{
    public class RegisterParentDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = default!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}