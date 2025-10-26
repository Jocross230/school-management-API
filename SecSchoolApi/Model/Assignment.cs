namespace SecSchoolApi.Model
{
    public class Assignment
    {
        public Guid Id { get; set; }
        public Guid? StudentId { get; set; }
        public Guid? ClassId { get; set; }
        public Guid? TeacherId { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
