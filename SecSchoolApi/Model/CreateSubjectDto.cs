namespace SecSchoolApi.Model
{
    public class CreateSubjectDto
    {
        public string Name { get; set; } = default!;
        public string? Code { get; set; }
    }
}
