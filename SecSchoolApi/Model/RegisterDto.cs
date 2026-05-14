namespace SecSchoolApi.Model
{
    public class RegisterDto
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Role { get; set; } = "Parent"; // Parent | Teacher | Admin
        public Guid? SchoolId { get; set; }
    }
}
