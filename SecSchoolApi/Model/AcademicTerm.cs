namespace SecSchoolApi.Model
{
    public class AcademicTerm
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!; // e.g. 2025 Term 1
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCurrent { get; set; }
    }
}
