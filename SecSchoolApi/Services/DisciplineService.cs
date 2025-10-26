using SecSchoolApi.Data;
using SecSchoolApi.Interface;
using SecSchoolApi.Model;
using Microsoft.EntityFrameworkCore;

namespace SecSchoolApi.Services
{
    public class DisciplineService : IDisciplineService
    {
        private readonly SchoolDbContext _db;
        public DisciplineService(SchoolDbContext db) => _db = db;

        public async Task<DisciplineCase> CreateAsync(DisciplineCase dto, CancellationToken ct = default)
        {
            _db.DisciplineCases.Add(dto);
            await _db.SaveChangesAsync(ct);
            return dto;
        }

        public async Task<IEnumerable<DisciplineCase>> GetAsync(Guid? studentId, DisciplineStatus? status, CancellationToken ct = default)
        {
            var q = _db.DisciplineCases.AsQueryable();
            if (studentId.HasValue) q = q.Where(c => c.StudentId == studentId);
            if (status.HasValue) q = q.Where(c => c.Status == status);
            return await q.OrderByDescending(c => c.CreatedAt).ToListAsync(ct);
        }

        public async Task<DisciplineCase?> UpdateStatusAsync(Guid id, DisciplineStatus status, CancellationToken ct = default)
        {
            var c = await _db.DisciplineCases.FindAsync(new object[] { id }, ct);
            if (c == null) return null;
            c.Status = status;
            if (status == DisciplineStatus.Resolved) c.ResolvedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return c;
        }
    }
}
