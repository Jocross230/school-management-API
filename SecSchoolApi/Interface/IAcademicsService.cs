using SecSchoolApi.Model;

namespace SecSchoolApi.Interface
{
    public interface IAcademicsService
    {
        Task<IEnumerable<AcademicTerm>> GetTermsAsync(CancellationToken ct = default);
        Task<AcademicTerm> CreateTermAsync(AcademicTerm term, CancellationToken ct = default);
        Task SetCurrentTermAsync(Guid termId, CancellationToken ct = default);

        Task<IEnumerable<Subject>> GetSubjectsAsync(CancellationToken ct = default);
        Task<Subject> CreateSubjectAsync(Subject subject, CancellationToken ct = default);

        Task<StudentClassEnrollment> EnrollAsync(Guid studentId, Guid classId, CancellationToken ct = default);
        Task<IEnumerable<StudentClassEnrollment>> GetEnrollmentsAsync(Guid studentId, CancellationToken ct = default);
    }
}