using System.ComponentModel.DataAnnotations;

namespace SecSchoolApi.Model
{
    public class CreateTermDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = default!;

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public bool IsCurrent { get; set; }
    }
}