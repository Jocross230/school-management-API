using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecSchoolApi.Interface;
using SecSchoolApi.Model;
using System.Data;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace SecSchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;

        public StudentsController(IStudentService studentService, IMapper mapper)
        {
            _studentService = studentService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        [SwaggerOperation(Summary = "List students")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var paged = await _studentService.GetPagedAsync(page, pageSize, ct);
            return Ok(paged);
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null) return NotFound();
            return Ok(_mapper.Map<StudentModel>(student));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] StudentModel dto)
        {
            var entity = _mapper.Map<StudentModel>(dto);
            var created = await _studentService.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<StudentModel>(created));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] StudentModel dto)
        {
            var ent = _mapper.Map<StudentModel>(dto);
            var updated = await _studentService.UpdateAsync(id, ent);
            if (updated == null) return NotFound();
            return Ok(_mapper.Map<StudentModel>(updated));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var ok = await _studentService.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        [HttpGet("{id:guid}/results")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        [SwaggerOperation(Summary = "Get student results", Description = "Filter by term/subject and paginate; parents only see published.")]
        public async Task<IActionResult> GetResults(
            Guid id,
            [FromQuery] string? term,
            [FromQuery] string? subject,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            bool? publishedOnly = User.IsInRole("Parent") ? true : (bool?)null;
            var results = await _studentService.GetResultsAsync(id, term, subject, publishedOnly, page, pageSize, ct);
            return Ok(results);
        }

        [HttpGet("{id:guid}/attendance")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<IActionResult> GetAttendance(Guid id)
        {
            var attendance = await _studentService.GetAttendanceAsync(id);
            return Ok(attendance);
        }

        [HttpPost("{id:guid}/accommodation/requests")]
        [Authorize(Roles = "Student,Parent,Admin,Teacher")]
        [SwaggerOperation(Summary = "Request accommodation", Description = "Student requests hostel allocation. Requires role: Student/Parent/Teacher/Admin.")]
        [SwaggerRequestExample(typeof(AccommodationRequestDto), typeof(SecSchoolApi.Swagger.AccommodationRequestDtoExample))]
        [SwaggerResponse(200, "Created request", typeof(AccommodationRequest))]
        [SwaggerResponseExample(200, typeof(SecSchoolApi.Swagger.AccommodationRequestExample))]
        public async Task<IActionResult> RequestAccommodation([FromServices] IAccommodationService accommodation, Guid id, [FromBody] AccommodationRequestDto dto, CancellationToken ct)
        {
            var created = await accommodation.RequestAsync(id, dto.Remark, ct);
            return Ok(created);
        }

        [HttpGet("{id:guid}/accommodation/requests")]
        [Authorize(Roles = "Student,Parent,Admin,Teacher")]
        [SwaggerOperation(Summary = "List accommodation requests for a student")]
        public async Task<IActionResult> GetAccommodationRequests([FromServices] IAccommodationService accommodation, Guid id, CancellationToken ct)
        {
            var list = await accommodation.GetStudentRequestsAsync(id, ct);
            return Ok(list);
        }
    }
}
