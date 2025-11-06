using SecSchoolApi.Interface;
using SecSchoolApi.Model;
using SecSchoolApi.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace SecSchoolApi.Services
{
    public class AdminService : IAdminService
    {
        private readonly SchoolDbContext _db;
        public AdminService(SchoolDbContext db) => _db = db;

        public async Task<StudentModel> RegisterStudentAsync(StudentModel student)
        {
            var parent = await _db.Parents.FindAsync(student.ParentId);
            if (parent == null)
                throw new Exception("Parent not found");

            student.Parent = parent;

            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            return student;
        }
        //business logic for registering a teacher
        public async Task<Teacher> RegisterTeacherAsync(Teacher teacher)
        {
            _db.Teachers.Add(teacher);
            await _db.SaveChangesAsync();
            return teacher;
        }

        public async Task<bool> UploadResultsAsync(IEnumerable<Result> results)
        {
            _db.Results.AddRange(results);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<AnnouncementModel> PublishAnnouncementAsync(AnnouncementModel announcement)
        {
            announcement.Id = Guid.NewGuid();
            announcement.Date = DateTime.UtcNow;
            _db.Announcements.Add(announcement);
            await _db.SaveChangesAsync();
            return announcement;
        }

        public async Task<object> GetFeeReportAsync()
        {
            var total = await _db.Payments.SumAsync(p => (decimal?)p.Amount) ?? 0m;
            var count = await _db.Payments.CountAsync();
            return new { TotalCollected = total, PaymentCount = count };
        }

        public async Task<object> GetPerformanceReportAsync()
        {
            var avgPerSubject = await _db.Results
                .GroupBy(r => r.Subject)
                .Select(g => new { Subject = g.Key, AvgScore = g.Average(r => r.Score) })
                .ToListAsync();
            return avgPerSubject;
        }

        public async Task<object> GetAttendanceReportAsync()
        {
            var totalRecords = await _db.Attendance.CountAsync();
            var presentCount = await _db.Attendance.CountAsync(a => a.IsPresent);
            return new { Total = totalRecords, Present = presentCount, Absent = totalRecords - presentCount };
        }
        public async Task<Admin> RegisterAdminAsync(Admin admin)
        {
            // Idempotency: if an Admin already exists for this identity user or email, return/undelete it.
            var existing = await _db.Admins.IgnoreQueryFilters()
                .FirstOrDefaultAsync(a => (admin.ApplicationUserId != null && a.ApplicationUserId == admin.ApplicationUserId)
                                          || a.Email == admin.Email);
            if (existing != null)
            {
                if (existing.IsDeleted)
                {
                    existing.IsDeleted = false;
                    existing.DeletedAt = null;
                    existing.FullName = admin.FullName;
                    existing.PhoneNumber = admin.PhoneNumber;
                    if (existing.ApplicationUserId == null && admin.ApplicationUserId != null)
                        existing.ApplicationUserId = admin.ApplicationUserId;
                    await _db.SaveChangesAsync();
                }
                return existing;
            }

            // Always generate a new Id to avoid client-sent duplicates
            admin.Id = Guid.NewGuid();
            _db.Admins.Add(admin);
            await _db.SaveChangesAsync();
            return admin;
        }

        public async Task<IEnumerable<Admin>> GetAllAsync()
        {
            return await _db.Admins.ToListAsync();
        }

        public async Task<Admin?> GetByIdAsync(Guid id)
        {
            return await _db.Admins.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Admin?> UpdateAsync(Guid id, Admin updated)
        {
            var existing = await _db.Admins.FindAsync(id);
            if (existing == null) return null;

            existing.FullName = updated.FullName;
            existing.Email = updated.Email;
            existing.PhoneNumber = updated.PhoneNumber;

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _db.Admins.FirstOrDefaultAsync(a => a.Id == id);
            if (existing == null) return false;

            existing.IsDeleted = true;
            existing.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreAsync(Guid id)
        {
            var existing = await _db.Admins.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Id == id);
            if (existing == null || !existing.IsDeleted) return false;

            existing.IsDeleted = false;
            existing.DeletedAt = null;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteByUserIdAsync(Guid userId)
        {
            var existing = await _db.Admins.FirstOrDefaultAsync(a => a.ApplicationUserId == userId);
            if (existing == null) return false;
            existing.IsDeleted = true;
            existing.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreByUserIdAsync(Guid userId)
        {
            var existing = await _db.Admins.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.ApplicationUserId == userId);
            if (existing == null || !existing.IsDeleted) return false;
            existing.IsDeleted = false;
            existing.DeletedAt = null;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
