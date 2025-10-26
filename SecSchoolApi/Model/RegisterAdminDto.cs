namespace SecSchoolApi.Model
{
    public class RegisterAdminDto
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? PhoneNumber { get; set; }
    }
}
