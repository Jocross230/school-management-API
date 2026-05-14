using SecSchoolApi.Model;

namespace SecSchoolApi.Interface
{
    public interface IAdminService
    {
        Task<StudentModel> RegisterStudentAsync(StudentModel student);
        Task<Teacher> RegisterTeacherAsync(Teacher teacher);
        Task<bool> UploadResultsAsync(IEnumerable<Result> results);
        Task<AnnouncementModel> PublishAnnouncementAsync(AnnouncementModel announcement);

        Task<object> GetFeeReportAsync();
        Task<object> GetPerformanceReportAsync();
        Task<object> GetAttendanceReportAsync();
        Task<Admin> RegisterAdminAsync(Admin admin);
        Task<IEnumerable<Admin>> GetAllAsync();
        Task<Admin?> GetByIdAsync(Guid id);
        Task<Admin?> UpdateAsync(Guid id, Admin updated);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> RestoreAsync(Guid id);
        Task<bool> DeleteByUserIdAsync(Guid userId);
        Task<bool> RestoreByUserIdAsync(Guid userId);
    }
}
