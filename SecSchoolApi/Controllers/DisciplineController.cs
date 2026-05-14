using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecSchoolApi.Interface;
using SecSchoolApi.Model;

namespace SecSchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DisciplineController : ControllerBase
    {
        private readonly IDisciplineService _service;
        public DisciplineController(IDisciplineService service) => _service = service;

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Create([FromBody] DisciplineCase dto, CancellationToken ct)
            => Ok(await _service.CreateAsync(dto, ct));

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<IActionResult> Get([FromQuery] Guid? studentId, [FromQuery] DisciplineStatus? status, CancellationToken ct)
            => Ok(await _service.GetAsync(studentId, status, ct));

        [HttpPost("{id:guid}/status")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] DisciplineStatus status, CancellationToken ct)
        {
            var updated = await _service.UpdateStatusAsync(id, status, ct);
            return updated == null ? NotFound() : Ok(updated);
        }
    }
}
