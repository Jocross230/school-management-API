namespace SecSchoolApi.Model
{
    public class AttendanceModel
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }

    }
}
