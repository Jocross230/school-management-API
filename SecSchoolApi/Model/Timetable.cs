namespace SecSchoolApi.Model
{
    public class Timetable
    {
        public Guid Id { get; set; }
        public Guid ClassId { get; set; }
        public string DayOfWeek { get; set; } = default!;
        public string Period { get; set; } = default!; // e.g. "08:00-09:00"
        public string Subject { get; set; } = default!;
        public Guid TeacherId { get; set; }
    }
}
