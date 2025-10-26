using SecSchoolApi.Data;
using SecSchoolApi.Interface;
using SecSchoolApi.Model;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace SecSchoolApi.Services
{
    public class AppControlService : IAppControlService
    {
        private readonly SchoolDbContext _db;
        private static readonly Stopwatch _uptime = Stopwatch.StartNew();
        public AppControlService(SchoolDbContext db) => _db = db;

        public string GetVersion() => typeof(AppControlService).Assembly.GetName().Version?.ToString() ?? "unknown";
        public DateTime GetUptime() => DateTime.UtcNow - _uptime.Elapsed;

        public async Task<bool> GetMaintenanceAsync(CancellationToken ct = default)
        {
            var s = await _db.AppSettings.AsNoTracking().FirstOrDefaultAsync(x => x.Key == "maintenance", ct);
            return s?.Value == "true";
        }

        public async Task SetMaintenanceAsync(bool enabled, CancellationToken ct = default)
        {
            var s = await _db.AppSettings.FirstOrDefaultAsync(x => x.Key == "maintenance", ct);
            if (s == null)
            {
                _db.AppSettings.Add(new AppSetting { Key = "maintenance", Value = enabled ? "true" : "false" });
            }
            else
            {
                s.Value = enabled ? "true" : "false";
            }
            await _db.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<AppSetting>> GetSettingsAsync(string? prefix = null, CancellationToken ct = default)
        {
            var q = _db.AppSettings.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(prefix)) q = q.Where(s => s.Key.StartsWith(prefix));
            return await q.OrderBy(s => s.Key).ToListAsync(ct);
        }

        public async Task<AppSetting> SetSettingAsync(string key, string value, CancellationToken ct = default)
        {
            var s = await _db.AppSettings.FirstOrDefaultAsync(x => x.Key == key, ct);
            if (s == null)
            {
                s = new AppSetting { Key = key, Value = value };
                _db.AppSettings.Add(s);
            }
            else
            {
                s.Value = value;
            }
            await _db.SaveChangesAsync(ct);
            return s;
        }
    }
}
