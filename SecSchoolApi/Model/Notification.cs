namespace SecSchoolApi.Model
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Channel { get; set; } = default!; // "sms","email","push"
        public string Message { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDelivered { get; set; }
    }
}
