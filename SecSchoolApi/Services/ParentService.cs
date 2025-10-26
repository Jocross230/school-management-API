using SecSchoolApi.Data;
using SecSchoolApi.Interface;
using SecSchoolApi.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace SecSchoolApi.Services
{
    public class ParentService : IParentService
    {
        private readonly SchoolDbContext _db;
        public ParentService(SchoolDbContext db) => _db = db;

        public async Task<ParentModel> CreateAsync(ParentModel parent, CancellationToken ct = default)
        {
            _db.Parents.Add(parent);
            await _db.SaveChangesAsync(ct);
            return parent;
        }

        public async Task<IEnumerable<ParentModel>> GetAllAsync(CancellationToken ct = default) =>
            await _db.Parents.ToListAsync(ct);

        public async Task<ParentModel?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            await _db.Parents.FindAsync(new object[] { id }, ct);

        public async Task<ParentModel?> UpdateAsync(Guid id, ParentModel parent, CancellationToken ct = default)
        {
            var existing = await _db.Parents.FindAsync(new object[] { id }, ct);
            if (existing == null) return null;
            existing.FullName = parent.FullName;
            existing.Email = parent.Email;
            await _db.SaveChangesAsync(ct);
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var existing = await _db.Parents.FindAsync(new object[] { id }, ct);
            if (existing == null) return false;
            _db.Parents.Remove(existing);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<IEnumerable<StudentModel>> GetChildrenAsync(Guid parentId, CancellationToken ct = default) =>
            await _db.Students.Where(s => s.ParentId == parentId).ToListAsync(ct);

        public async Task<IEnumerable<AnnouncementModel>> GetNotificationsAsync(Guid parentId, CancellationToken ct = default)
        {
            return await _db.Announcements.OrderByDescending(a => a.Date).ToListAsync(ct);
        }

        public async Task<IEnumerable<FeePayment>> GetPaymentHistoryAsync(Guid parentId, CancellationToken ct = default) =>
            await _db.Payments.Where(p => p.ParentId == parentId).OrderByDescending(p => p.Date).ToListAsync(ct);

        public async Task<Message> SendMessageAsync(Guid parentId, Message message, CancellationToken ct = default)
        {
            message.FromUserId = parentId;
            message.SentAt = DateTime.UtcNow;
            _db.Set<Message>().Add(message);
            await _db.SaveChangesAsync(ct);
            return message;
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync(Guid parentId, CancellationToken ct = default) =>
            await _db.Set<Message>()
                     .Where(m => m.FromUserId == parentId || m.ToUserId == parentId)
                     .OrderByDescending(m => m.SentAt)
                     .ToListAsync(ct);
    }
}
