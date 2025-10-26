using SecSchoolApi.Data;
using SecSchoolApi.Interface;
using SecSchoolApi.Model;
using Microsoft.EntityFrameworkCore;

namespace SecSchoolApi.Services
{
    public class BillingService : IBillingService
    {
        private readonly SchoolDbContext _db;
        public BillingService(SchoolDbContext db) => _db = db;

        public async Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken ct = default)
        {
            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync(ct);
            return invoice;
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesAsync(Guid? parentId, Guid? studentId, InvoiceStatus? status, CancellationToken ct = default)
        {
            var q = _db.Invoices.AsQueryable();
            if (parentId.HasValue) q = q.Where(i => i.ParentId == parentId);
            if (studentId.HasValue) q = q.Where(i => i.StudentId == studentId);
            if (status.HasValue) q = q.Where(i => i.Status == status);
            return await q.OrderByDescending(i => i.CreatedAt).ToListAsync(ct);
        }

        public async Task<Invoice?> MarkPaidAsync(Guid invoiceId, CancellationToken ct = default)
        {
            var inv = await _db.Invoices.FindAsync(new object[] { invoiceId }, ct);
            if (inv == null) return null;
            inv.Status = InvoiceStatus.Paid;
            inv.PaidAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return inv;
        }

        public async Task<Invoice?> RefundAsync(Guid invoiceId, CancellationToken ct = default)
        {
            var inv = await _db.Invoices.FindAsync(new object[] { invoiceId }, ct);
            if (inv == null) return null;
            inv.Status = InvoiceStatus.Refunded;
            await _db.SaveChangesAsync(ct);
            return inv;
        }
    }
}
