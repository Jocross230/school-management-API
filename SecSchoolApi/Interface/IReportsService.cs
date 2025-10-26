using SecSchoolApi.Model;

namespace SecSchoolApi.Interface
{
    public interface IReportsService
    {
        Task<object> GetReportCardAsync(Guid studentId, Guid termId, CancellationToken ct = default);
    }
}