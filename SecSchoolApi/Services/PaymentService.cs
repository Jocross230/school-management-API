using SecSchoolApi.Interface;
using SecSchoolApi.Data;
using SecSchoolApi.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace SecSchoolApi.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly SchoolDbContext _db;
        public PaymentService(SchoolDbContext db) => _db = db;

        public async Task<FeePayment> InitiatePaymentAsync(FeePayment payment)
        {
            payment.Date = DateTime.UtcNow;
            payment.Reference = payment.Reference ?? Guid.NewGuid().ToString("N");
            payment.IsVerified = false;
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();
            return payment;
        }

        public async Task<IEnumerable<FeePayment>> GetPaymentHistoryAsync(Guid parentId) =>
            await _db.Payments.Where(p => p.ParentId == parentId).OrderByDescending(p => p.Date).ToListAsync();

        public async Task<bool> VerifyPaymentAsync(string reference)
        {
            var p = await _db.Payments.FirstOrDefaultAsync(x => x.Reference == reference);
            if (p == null) return false;
            p.IsVerified = true;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<object> GetPaymentReportAsync()
        {
            var total = await _db.Payments.Where(p => p.IsVerified).SumAsync(p => (decimal?)p.Amount) ?? 0m;
            var byDate = await _db.Payments
                .Where(p => p.IsVerified)
                .GroupBy(p => p.Date.Date)
                .Select(g => new { Date = g.Key, Collected = g.Sum(x => x.Amount) })
                .OrderByDescending(x => x.Date)
                .ToListAsync();
            return new { TotalCollected = total, ByDate = byDate };
        }
    }
}
