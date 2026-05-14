namespace SecSchoolApi.Model
{
    public class Class
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!; // e.g. "JSS1A"
        public Guid TeacherId { get; set; }
    }
}
