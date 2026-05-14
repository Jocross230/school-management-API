using SecSchoolApi.Model;


public interface IAcademicsService
{
    Task<IEnumerable<AcademicTerm>> GetTermsAsync(CancellationToken ct = default);
    Task<AcademicTerm?> GetTermByIdAsync(Guid termId, CancellationToken ct = default);
    Task<AcademicTerm> CreateTermAsync(AcademicTerm term, CancellationToken ct = default);
    Task<AcademicTerm?> UpdateTermAsync(Guid termId, AcademicTerm term, CancellationToken ct = default);
    Task<bool> DeleteTermAsync(Guid termId, CancellationToken ct = default);
    Task<bool> SetCurrentTermAsync(Guid termId, CancellationToken ct = default); // changed to return bool

    Task<IEnumerable<Subject>> GetSubjectsAsync(CancellationToken ct = default);
    Task<Subject?> GetSubjectByIdAsync(Guid subjectId, CancellationToken ct = default);
    Task<Subject> CreateSubjectAsync(Subject subject, CancellationToken ct = default);
    Task<Subject?> UpdateSubjectAsync(Guid subjectId, Subject subject, CancellationToken ct = default);
    Task<bool> DeleteSubjectAsync(Guid subjectId, CancellationToken ct = default);

    Task<StudentClassEnrollment> EnrollAsync(Guid studentId, Guid classId, CancellationToken ct = default);
    Task<IEnumerable<StudentClassEnrollment>> GetEnrollmentsAsync(Guid studentId, CancellationToken ct = default);
}