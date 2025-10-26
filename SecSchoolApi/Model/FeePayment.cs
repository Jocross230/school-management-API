namespace SecSchoolApi.Model
{
    public class FeePayment
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; } = default!;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public bool IsVerified { get; set; }
    }
}
