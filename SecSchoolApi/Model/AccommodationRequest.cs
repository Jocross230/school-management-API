namespace SecSchoolApi.Model
{
    public class AccommodationRequest
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public AccommodationStatus Status { get; set; } = AccommodationStatus.Pending;
        public Guid? RoomId { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        public string? Remark { get; set; }
    }
}
