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
        [SwaggerOperation(OperationId = "Term_List", Summary = "List academic terms")]
        public async Task<IActionResult> GetTerms(CancellationToken ct) => Ok(await _service.GetTermsAsync(ct));

        [HttpGet("terms/{termId:guid}")]
        [Authorize(Roles = "Admin,Teacher")]
        [SwaggerOperation(OperationId = "Term_GetById", Summary = "Get academic term by id")]
        public async Task<IActionResult> GetTermById(Guid termId, CancellationToken ct)
            => (await _service.GetTermByIdAsync(termId, ct)) is AcademicTerm t ? Ok(t) : NotFound();

        [HttpPost("terms")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(OperationId = "Term_Create", Summary = "Create academic term")]
        public async Task<IActionResult> CreateTerm([FromBody] CreateTermDto dto, CancellationToken ct)
            => Ok(await _service.CreateTermAsync(new AcademicTerm { Name = dto.Name, StartDate = dto.StartDate, EndDate = dto.EndDate, IsCurrent = dto.IsCurrent }, ct));

        [HttpPut("terms/{termId:guid}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(OperationId = "Term_Update", Summary = "Update academic term")] 
        public async Task<IActionResult> UpdateTerm(Guid termId, [FromBody] AcademicTerm dto, CancellationToken ct)
        {
            var updated = await _service.UpdateTermAsync(termId, dto, ct);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("terms/{termId:guid}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(OperationId = "Term_Delete", Summary = "Delete academic term (blocked if current or has enrollments)")] 
        public async Task<IActionResult> DeleteTerm(Guid termId, CancellationToken ct)
        {
            var ok = await _service.DeleteTermAsync(termId, ct);
            return ok ? NoContent() : BadRequest("Cannot delete current term or term with enrollments, or term not found.");
        }

        [HttpPost("terms/{termId:guid}/current")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(OperationId = "Term_SetCurrent", Summary = "Set current academic term")]
        public async Task<IActionResult> SetCurrent(Guid termId, CancellationToken ct)
        {
            await _service.SetCurrentTermAsync(termId, ct);
            return NoContent();
        }

        [HttpGet("subjects")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetSubjects(CancellationToken ct) => Ok(await _service.GetSubjectsAsync(ct));

        [HttpGet("subjects/{subjectId:guid}")]
        [Authorize(Roles = "Admin,Teacher")]
        [SwaggerOperation(OperationId = "Subject_GetById", Summary = "Get subject by id")]
        public async Task<IActionResult> GetSubjectById(Guid subjectId, CancellationToken ct)
            => (await _service.GetSubjectByIdAsync(subjectId, ct)) is Subject s ? Ok(s) : NotFound();

        [HttpPost("subjects")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(OperationId = "Subject_Create", Summary = "Create subject")]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto dto, CancellationToken ct)
            => Ok(await _service.CreateSubjectAsync(new Subject { Name = dto.Name, Code = dto.Code }, ct));

        [HttpPut("subjects/{subjectId:guid}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(OperationId = "Subject_Update", Summary = "Update subject")]
        public async Task<IActionResult> UpdateSubject(Guid subjectId, [FromBody] UpdateSubjectDto dto, CancellationToken ct)
        {
            var updated = await _service.UpdateSubjectAsync(subjectId, new Subject { Name = dto.Name, Code = dto.Code }, ct);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("subjects/{subjectId:guid}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(OperationId = "Subject_Delete", Summary = "Delete subject")]
        public async Task<IActionResult> DeleteSubject(Guid subjectId, CancellationToken ct)
        {
            var ok = await _service.DeleteSubjectAsync(subjectId, ct);
            return ok ? NoContent() : NotFound();
        }

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
