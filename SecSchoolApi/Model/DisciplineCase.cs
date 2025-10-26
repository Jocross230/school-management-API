namespace SecSchoolApi.Model
{
    public class DisciplineCase
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DisciplineStatus Status { get; set; } = DisciplineStatus.Open;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }
    }
}
