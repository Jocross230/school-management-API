using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecSchoolApi.Model
{
    public class Room
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = default!;
        public string? Hostel { get; set; }
        public int Capacity { get; set; }
        public int Occupied { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

        [NotMapped]
        public int Available => Math.Max(0, Capacity - Occupied);
    }
}
