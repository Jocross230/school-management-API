namespace SecSchoolApi.Model
{
    public class Teacher
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Subject { get; set; } = default!;

    }
}
