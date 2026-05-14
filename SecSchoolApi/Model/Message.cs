namespace SecSchoolApi.Model
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
        public string Subject { get; set; } = default!;
        public string Body { get; set; } = default!;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
    }
}
