namespace SecSchoolApi.Model
{
    public class StudentClassEnrollment
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid ClassId { get; set; }
        public Guid TermId { get; set; }
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    }
}
