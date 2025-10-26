namespace SecSchoolApi.Model
{
    public class StudentModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public string Class { get; set; } = default!;
        public string? HealthIssue { get; set; }

        public Guid ParentId { get; set; }
        public ParentModel? Parent { get; set; }
    }
}
