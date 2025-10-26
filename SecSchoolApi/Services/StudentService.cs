using SecSchoolApi.Data;
using SecSchoolApi.Model;
using SecSchoolApi.Interface;
using System;
using System.Xml;
using Microsoft.EntityFrameworkCore;

namespace SecSchoolApi.Services
{
    public class StudentService : IStudentService
    {
        private readonly SchoolDbContext _context;
        public StudentService(SchoolDbContext context) => _context = context;

        public async Task<IEnumerable<StudentModel>> GetAllAsync() =>
            await _context.Students.Include(s => s.Parent).ToListAsync();

        public async Task<PagedResult<StudentModel>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;
            var baseQuery = _context.Students.Include(s => s.Parent).AsQueryable();
            var total = await baseQuery.CountAsync(ct);
            var items = await baseQuery
                .OrderBy(s => s.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);
            return new PagedResult<StudentModel> { Page = page, PageSize = pageSize, TotalCount = total, Items = items };
        }

        public async Task<StudentModel?> GetByIdAsync(Guid id) =>
            await _context.Students.Include(s => s.Parent).FirstOrDefaultAsync(s => s.Id == id);

        public async Task<StudentModel> CreateAsync(StudentModel student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<StudentModel?> UpdateAsync(Guid id, StudentModel updated)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return null;

            student.FullName = updated.FullName;
            student.Class = updated.Class;
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return false;
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AttendanceModel>> GetAttendanceAsync(Guid studentId) =>
            await _context.Attendance.Where(a => a.StudentId == studentId)
                                     .OrderByDescending(a => a.Date)
                                     .ToListAsync();

        public async Task<IEnumerable<Result>> GetResultsAsync(Guid studentId) =>
            await _context.Results.Where(r => r.StudentId == studentId)
                                   .OrderByDescending(r => r.Term)
                                   .ToListAsync();

        public async Task<IEnumerable<Result>> GetResultsAsync(Guid studentId, string? term, string? subject, bool? publishedOnly, int page, int pageSize, CancellationToken ct = default)
        {
            var q = _context.Results.Where(r => r.StudentId == studentId);
            if (!string.IsNullOrWhiteSpace(term)) q = q.Where(r => r.Term == term);
            if (!string.IsNullOrWhiteSpace(subject)) q = q.Where(r => r.Subject == subject);
            if (publishedOnly == true) q = q.Where(r => r.IsPublished);
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;
            return await q.OrderByDescending(r => r.Term)
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync(ct);
        }

    }
}

