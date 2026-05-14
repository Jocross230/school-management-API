using SecSchoolApi.Interface;
using SecSchoolApi.Model;
using SecSchoolApi.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace SecSchoolApi.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly SchoolDbContext _db;
        public TeacherService(SchoolDbContext db) => _db = db;

        public async Task<IEnumerable<Teacher>> GetAllAsync() =>
            await _db.Teachers.ToListAsync();

        public async Task<PagedResult<Teacher>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;
            var total = await _db.Teachers.CountAsync(ct);
            var items = await _db.Teachers
                                 .OrderBy(t => t.FullName)
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync(ct);
            return new PagedResult<Teacher> { Page = page, PageSize = pageSize, TotalCount = total, Items = items };
        }

        public async Task<Teacher?> GetByIdAsync(Guid id) =>
            await _db.Teachers.FindAsync(id);

        public async Task<Teacher> CreateAsync(Teacher teacher)
        {
            _db.Teachers.Add(teacher);
            await _db.SaveChangesAsync();
            return teacher;
        }

        public async Task<Teacher?> UpdateAsync(Guid id, Teacher teacher)
        {
            var t = await _db.Teachers.FindAsync(id);
            if (t == null) return null;
            t.FullName = teacher.FullName;
            t.Subject = teacher.Subject;
            await _db.SaveChangesAsync();
            return t;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var t = await _db.Teachers.FindAsync(id);
            if (t == null) return false;
            _db.Teachers.Remove(t);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAttendanceAsync(Guid teacherId, AttendanceModel attendance)
        {
            _db.Attendance.Add(attendance);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<Result> UploadResultAsync(Guid teacherId, Result result)
        {
            _db.Results.Add(result);
            await _db.SaveChangesAsync();
            return result;
        }

        public async Task<Result?> PublishResultAsync(Guid resultId, bool isPublished, CancellationToken ct = default)
        {
            var r = await _db.Results.FindAsync(new object[] { resultId }, ct);
            if (r == null) return null;
            r.IsPublished = isPublished;
            await _db.SaveChangesAsync(ct);
            return r;
        }

        public async Task<Assignment> AssignHomeworkAsync(Guid teacherId, Assignment assignment)
        {
            _db.Set<Assignment>().Add(assignment);
            await _db.SaveChangesAsync();
            return assignment;
        }

        public async Task<IEnumerable<Assignment>> GetAssignmentsAsync(Guid? classId, Guid? studentId, DateTime? dueFrom, DateTime? dueTo, int page, int pageSize, CancellationToken ct = default)
        {
            var q = _db.Set<Assignment>().AsQueryable();
            if (classId.HasValue && classId != Guid.Empty) q = q.Where(a => a.ClassId == classId);
            if (studentId.HasValue && studentId != Guid.Empty) q = q.Where(a => a.StudentId == studentId);
            if (dueFrom.HasValue) q = q.Where(a => a.DueDate >= dueFrom.Value);
            if (dueTo.HasValue) q = q.Where(a => a.DueDate <= dueTo.Value);
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;
            return await q.OrderByDescending(a => a.CreatedAt)
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync(ct);
        }

        public async Task<IEnumerable<Class>> GetClassesAsync(Guid teacherId) =>
            await _db.Set<Class>().Where(c => c.TeacherId == teacherId).ToListAsync();
    }
}
