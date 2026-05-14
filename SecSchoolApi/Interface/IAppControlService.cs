using SecSchoolApi.Model;

namespace SecSchoolApi.Interface
{
    public interface IAppControlService
    {
        Task<bool> GetMaintenanceAsync(CancellationToken ct = default);
        Task SetMaintenanceAsync(bool enabled, CancellationToken ct = default);
        Task<IEnumerable<AppSetting>> GetSettingsAsync(string? prefix = null, CancellationToken ct = default);
        Task<AppSetting> SetSettingAsync(string key, string value, CancellationToken ct = default);
        string GetVersion();
        DateTime GetUptime();
    }
}