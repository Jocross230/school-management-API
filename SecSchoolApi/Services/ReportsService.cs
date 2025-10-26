using SecSchoolApi.Data;
using SecSchoolApi.Interface;
using Microsoft.EntityFrameworkCore;

namespace SecSchoolApi.Services
{
    public class ReportsService : IReportsService
    {
        private readonly SchoolDbContext _db;
        public ReportsService(SchoolDbContext db) => _db = db;

        public async Task<object> GetReportCardAsync(Guid studentId, Guid termId, CancellationToken ct = default)
        {
            var results = await _db.Results.Where(r => r.StudentId == studentId).ToListAsync(ct);
            var grouped = results.GroupBy(r => r.Subject)
                                 .Select(g => new { Subject = g.Key, Average = g.Average(x => x.Score), Highest = g.Max(x => x.Score), Lowest = g.Min(x => x.Score) })
                                 .OrderBy(x => x.Subject)
                                 .ToList();
            var avg = results.Any() ? results.Average(r => r.Score) : 0;
            return new { StudentId = studentId, TermId = termId, Average = avg, Subjects = grouped };
        }
    }
}
