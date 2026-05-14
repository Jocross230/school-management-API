using System.ComponentModel.DataAnnotations;

namespace SecSchoolApi.Model
{
    public class EmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        public string Subject { get; set; } = default!;

        [Required]
        public string Body { get; set; } = default!;
    }
}