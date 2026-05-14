namespace SecSchoolApi.Model
{
    public class Branding
    {
        public Guid Id { get; set; }
        public Guid SchoolId { get; set; }
        public string LogoUrl { get; set; } = default!;
        public string PrimaryColor { get; set; } = default!;
        public string SecondaryColor { get; set; } = default!;
    }
}
