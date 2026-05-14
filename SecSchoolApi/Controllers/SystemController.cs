using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecSchoolApi.Interface;
using Swashbuckle.AspNetCore.Annotations;

namespace SecSchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        [HttpGet("health")] 
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Health check")] 
        public IActionResult Health([FromServices] IAppControlService control)
        {
            return Ok(new { status = "ok", version = control.GetVersion(), startedAtUtc = control.GetUptime() });
        }

        [HttpGet("maintenance")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Get maintenance mode")] 
        public async Task<IActionResult> GetMaintenance([FromServices] IAppControlService control, CancellationToken ct)
        {
            var enabled = await control.GetMaintenanceAsync(ct);
            return Ok(new { maintenance = enabled });
        }

        [HttpPost("maintenance")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Set maintenance mode")] 
        public async Task<IActionResult> SetMaintenance([FromServices] IAppControlService control, [FromQuery] bool enabled, CancellationToken ct)
        {
            await control.SetMaintenanceAsync(enabled, ct);
            return Ok(new { maintenance = enabled });
        }

        [HttpGet("settings")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "List application settings")] 
        public async Task<IActionResult> GetSettings([FromServices] IAppControlService control, [FromQuery] string? prefix, CancellationToken ct)
        {
            var list = await control.GetSettingsAsync(prefix, ct);
            return Ok(list);
        }

        [HttpPut("settings/{key}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Set application setting")] 
        public async Task<IActionResult> SetSetting([FromServices] IAppControlService control, string key, [FromBody] string value, CancellationToken ct)
        {
            var saved = await control.SetSettingAsync(key, value, ct);
            return Ok(saved);
        }
    }
}
