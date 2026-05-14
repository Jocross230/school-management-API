namespace SecSchoolApi.Model
{
    public class RegisterStudentDto
    {
        public string FullName { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public string Class { get; set; } = default!;
        public string? HealthIssue { get; set; }
        public Guid ParentId { get; set; }
    }
}
