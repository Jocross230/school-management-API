using SecSchoolApi.Model;

namespace SecSchoolApi.Interface
{
    public interface IBillingService
    {
        Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken ct = default);
        Task<IEnumerable<Invoice>> GetInvoicesAsync(Guid? parentId, Guid? studentId, InvoiceStatus? status, CancellationToken ct = default);
        Task<Invoice?> MarkPaidAsync(Guid invoiceId, CancellationToken ct = default);
        Task<Invoice?> RefundAsync(Guid invoiceId, CancellationToken ct = default);
    }
}