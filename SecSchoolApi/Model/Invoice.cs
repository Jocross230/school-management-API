namespace SecSchoolApi.Model
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? StudentId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
    }
}
