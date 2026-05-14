namespace SecSchoolApi.Model
{
    public class Result
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string Subject { get; set; } = default!;
        public int Score { get; set; }
        public string Term { get; set; } = default!;
        public Guid? TeacherId { get; set; }
        public string? Comment { get; set; }
        public bool IsPublished { get; set; }
    }
}
