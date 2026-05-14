namespace SecSchoolApi.Model
{
    public class Subject
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Code { get; set; }
    }
}
