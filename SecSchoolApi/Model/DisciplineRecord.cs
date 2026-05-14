namespace SecSchoolApi.Model
{
    public class DisciplineRecord
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string Issue { get; set; } = default!;
        public string ActionTaken { get; set; } = default!;
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
