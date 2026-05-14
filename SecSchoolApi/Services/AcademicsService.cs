using SecSchoolApi.Data;
using SecSchoolApi.Model;
using Microsoft.EntityFrameworkCore;

public class AcademicsService : IAcademicsService
{
    private readonly SchoolDbContext _db;
    public AcademicsService(SchoolDbContext db) => _db = db;

    public async Task<IEnumerable<AcademicTerm>> GetTermsAsync(CancellationToken ct = default) =>
        await _db.Terms.OrderByDescending(t => t.StartDate).ToListAsync(ct);

    public async Task<AcademicTerm?> GetTermByIdAsync(Guid termId, CancellationToken ct = default)
        => await _db.Terms.FindAsync(new object[] { termId }, ct);

    public async Task<AcademicTerm> CreateTermAsync(AcademicTerm term, CancellationToken ct = default)
    {
        if (term.StartDate > term.EndDate) throw new ArgumentException("StartDate must be before EndDate");

        if (term.IsCurrent)
        {
            var currentTerms = await _db.Terms.Where(x => x.IsCurrent).ToListAsync(ct);
            foreach (var t in currentTerms) t.IsCurrent = false;
        }

        term.Id = Guid.NewGuid();
        _db.Terms.Add(term);
        await _db.SaveChangesAsync(ct);
        return term;
    }

    public async Task<AcademicTerm?> UpdateTermAsync(Guid termId, AcademicTerm term, CancellationToken ct = default)
    {
        var existing = await _db.Terms.FindAsync(new object[] { termId }, ct);
        if (existing == null) return null;

        existing.Name = term.Name;
        existing.StartDate = term.StartDate;
        existing.EndDate = term.EndDate;

        if (term.IsCurrent && !existing.IsCurrent)
        {
            var currentTerms = await _db.Terms.Where(x => x.IsCurrent).ToListAsync(ct);
            foreach (var t in currentTerms) t.IsCurrent = false;
            existing.IsCurrent = true;
        }
        else if (!term.IsCurrent && existing.IsCurrent)
        {
            // keep at least one current; ignore attempt to unset current via update
        }

        await _db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> DeleteTermAsync(Guid termId, CancellationToken ct = default)
    {
        var existing = await _db.Terms.FindAsync(new object[] { termId }, ct);
        if (existing == null) return false;
        if (existing.IsCurrent) return false;
        var hasEnrollments = await _db.Enrollments.AnyAsync(e => e.TermId == termId, ct);
        if (hasEnrollments) return false;
        _db.Terms.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> SetCurrentTermAsync(Guid termId, CancellationToken ct = default)
    {
        var currentTerms = await _db.Terms.Where(t => t.IsCurrent).ToListAsync(ct);
        foreach (var t in currentTerms) t.IsCurrent = false;

        var current = await _db.Terms.FindAsync(new object[] { termId }, ct);
        if (current == null)
        {
            // nothing to set
            return false;
        }

        current.IsCurrent = true;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IEnumerable<Subject>> GetSubjectsAsync(CancellationToken ct = default) =>
        await _db.Subjects.OrderBy(s => s.Name).ToListAsync(ct);

    public async Task<Subject?> GetSubjectByIdAsync(Guid subjectId, CancellationToken ct = default)
        => await _db.Subjects.FindAsync(new object[] { subjectId }, ct);

    public async Task<Subject> CreateSubjectAsync(Subject subject, CancellationToken ct = default)
    {
        subject.Id = Guid.NewGuid();
        _db.Subjects.Add(subject);
        await _db.SaveChangesAsync(ct);
        return subject;
    }

    public async Task<Subject?> UpdateSubjectAsync(Guid subjectId, Subject subject, CancellationToken ct = default)
    {
        var existing = await _db.Subjects.FindAsync(new object[] { subjectId }, ct);
        if (existing == null) return null;
        existing.Name = subject.Name;
        existing.Code = subject.Code;
        await _db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> DeleteSubjectAsync(Guid subjectId, CancellationToken ct = default)
    {
        var existing = await _db.Subjects.FindAsync(new object[] { subjectId }, ct);
        if (existing == null) return false;
        _db.Subjects.Remove(existing);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<StudentClassEnrollment> EnrollAsync(Guid studentId, Guid classId, CancellationToken ct = default)
    {
        var term = await _db.Terms.FirstOrDefaultAsync(t => t.IsCurrent, ct);
        if (term == null) throw new InvalidOperationException("No current term set");
        var e = new StudentClassEnrollment { StudentId = studentId, ClassId = classId, TermId = term.Id };
        _db.Enrollments.Add(e);
        await _db.SaveChangesAsync(ct);
        return e;
    }

    public async Task<IEnumerable<StudentClassEnrollment>> GetEnrollmentsAsync(Guid studentId, CancellationToken ct = default) =>
        await _db.Enrollments.Where(e => e.StudentId == studentId).OrderByDescending(e => e.EnrolledAt).ToListAsync(ct);
}