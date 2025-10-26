namespace SecSchoolApi.Model
{
    public class AnnouncementModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string Message { get; set; } = default!;
        public DateTime Date { get; set; } = DateTime.UtcNow;

    }
}
