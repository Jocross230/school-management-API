using SecSchoolApi.Model;

namespace SecSchoolApi.Interface
{
    public interface ISchoolService
    {
        Task<IEnumerable<School>> GetAllAsync();
        Task<School?> GetByIdAsync(Guid id);
        Task<School> RegisterAsync(School school);
        Task<School?> UpdateAsync(Guid id, School school);
        Task<Branding?> GetBrandingAsync(Guid schoolId);
        Task<Branding> SetBrandingAsync(Guid schoolId, Branding branding, CancellationToken ct = default);
    }
}
