using SecSchoolApi.Model;

namespace SecSchoolApi.Interface
{
    public interface ITeacherService
    {
        Task<IEnumerable<Teacher>> GetAllAsync();
        Task<PagedResult<Teacher>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<Teacher?> GetByIdAsync(Guid id);
        Task<Teacher> CreateAsync(Teacher teacher);
        Task<Teacher?> UpdateAsync(Guid id, Teacher teacher);
        Task<bool> DeleteAsync(Guid id);

        Task<bool> MarkAttendanceAsync(Guid teacherId, AttendanceModel attendance);
        Task<Result> UploadResultAsync(Guid teacherId, Result result);
        Task<Result?> PublishResultAsync(Guid resultId, bool isPublished, CancellationToken ct = default);
        Task<Assignment> AssignHomeworkAsync(Guid teacherId, Assignment assignment);
        Task<IEnumerable<Assignment>> GetAssignmentsAsync(Guid? classId, Guid? studentId, DateTime? dueFrom, DateTime? dueTo, int page, int pageSize, CancellationToken ct = default);
        Task<IEnumerable<Class>> GetClassesAsync(Guid teacherId);
    }
}
