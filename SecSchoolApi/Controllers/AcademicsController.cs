using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecSchoolApi.Interface;
using SecSchoolApi.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace SecSchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AcademicsController : ControllerBase
    {
        private readonly IAcademicsService _service;
        public AcademicsController(IAcademicsService service) => _service = service;

        [HttpGet("terms")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetTerms(CancellationToken ct) => Ok(await _service.GetTermsAsync(ct));

        [HttpPost("terms")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTerm([FromBody] AcademicTerm dto, CancellationToken ct) => Ok(await _service.CreateTermAsync(dto, ct));

        [HttpPost("terms/{termId:guid}/current")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetCurrent(Guid termId, CancellationToken ct)
        {
            await _service.SetCurrentTermAsync(termId, ct);
            return NoContent();
        }

        [HttpGet("subjects")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetSubjects(CancellationToken ct) => Ok(await _service.GetSubjectsAsync(ct));

        [HttpPost("subjects")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSubject([FromBody] Subject dto, CancellationToken ct) => Ok(await _service.CreateSubjectAsync(dto, ct));

        [HttpPost("enrollments")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Enroll a student into a class for current term")]
        public async Task<IActionResult> Enroll([FromQuery] Guid studentId, [FromQuery] Guid classId, CancellationToken ct)
            => Ok(await _service.EnrollAsync(studentId, classId, ct));

        [HttpGet("students/{studentId:guid}/enrollments")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<IActionResult> GetEnrollments(Guid studentId, CancellationToken ct) => Ok(await _service.GetEnrollmentsAsync(studentId, ct));
    }
}
