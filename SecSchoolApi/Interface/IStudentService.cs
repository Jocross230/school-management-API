using SecSchoolApi.Model;
using System.Xml;

namespace SecSchoolApi.Interface
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentModel>> GetAllAsync();
        Task<PagedResult<StudentModel>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<StudentModel?> GetByIdAsync(Guid id);
        Task<StudentModel> CreateAsync(StudentModel student);
        Task<StudentModel?> UpdateAsync(Guid id, StudentModel student);
        Task<bool> DeleteAsync(Guid id);

        Task<IEnumerable<AttendanceModel>> GetAttendanceAsync(Guid studentId);
        Task<IEnumerable<Result>> GetResultsAsync(Guid studentId);
        Task<IEnumerable<Result>> GetResultsAsync(Guid studentId, string? term, string? subject, bool? publishedOnly, int page, int pageSize, CancellationToken ct = default);
    }
}
