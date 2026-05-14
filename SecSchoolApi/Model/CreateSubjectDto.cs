using System.ComponentModel.DataAnnotations;

namespace SecSchoolApi.Model
{
    public class CreateSubjectDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = default!;

        [StringLength(32)]
        public string? Code { get; set; }
    }
}