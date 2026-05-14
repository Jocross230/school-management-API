namespace SecSchoolApi.Model
{
    public class School
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string ContactEmail { get; set; } = default!;
        public Branding? Branding { get; set; }
    }
}
