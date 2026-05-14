using SecSchoolApi.Model;

namespace SecSchoolApi.Interface
{
    public interface IDisciplineService
    {
        Task<DisciplineCase> CreateAsync(DisciplineCase dto, CancellationToken ct = default);
        Task<IEnumerable<DisciplineCase>> GetAsync(Guid? studentId, DisciplineStatus? status, CancellationToken ct = default);
        Task<DisciplineCase?> UpdateStatusAsync(Guid id, DisciplineStatus status, CancellationToken ct = default);
    }
}