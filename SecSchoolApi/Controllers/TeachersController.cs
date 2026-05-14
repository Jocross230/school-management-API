using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecSchoolApi.Interface;
using SecSchoolApi.Model;
using System.Data;
using Swashbuckle.AspNetCore.Annotations;

namespace SecSchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;
        private readonly IMapper _mapper;

        public TeachersController(ITeacherService teacherService, IMapper mapper)
        {
            _teacherService = teacherService;
            _mapper = mapper;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "List teachers")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var paged = await _teacherService.GetPagedAsync(page, pageSize, ct);
            return Ok(paged);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Teacher dto)
        {
            var t = _mapper.Map<Teacher>(dto);
            var created = await _teacherService.CreateAsync(t);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, _mapper.Map<Teacher>(created));
        }

        [HttpPost("{teacherId:guid}/attendance")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> MarkAttendance(Guid teacherId, [FromBody] AttendanceModel dto)
        {
            var att = _mapper.Map<AttendanceModel>(dto);
            var ok = await _teacherService.MarkAttendanceAsync(teacherId, att);
            return ok ? Ok() : BadRequest();
        }

        [HttpPost("{teacherId:guid}/results")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> UploadResult(Guid teacherId, [FromBody] Result dto)
        {
            dto.TeacherId = teacherId;
            dto.IsPublished = false;
            var created = await _teacherService.UploadResultAsync(teacherId, dto);
            return Ok(created);
        }
        [HttpPost("{teacherId:guid}/assignments")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> CreateAssignment(Guid teacherId, [FromBody] Assignment dto)
        {
            dto.TeacherId = teacherId;
            var created = await _teacherService.AssignHomeworkAsync(teacherId, dto);
            return Ok(created);
        }

        [HttpPost("{teacherId:guid}/assignments/class/{classId:guid}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> CreateClassAssignment(Guid teacherId, Guid classId, [FromBody] Assignment dto)
        {
            dto.TeacherId = teacherId;
            dto.ClassId = classId;
            dto.StudentId = null;
            var created = await _teacherService.AssignHomeworkAsync(teacherId, dto);
            return Ok(created);
        }

        [HttpPost("{teacherId:guid}/assignments/all")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> CreateGlobalAssignment(Guid teacherId, [FromBody] Assignment dto)
        {
            dto.TeacherId = teacherId;
            dto.ClassId = null;
            dto.StudentId = null;
            var created = await _teacherService.AssignHomeworkAsync(teacherId, dto);
            return Ok(created);
        }

        [HttpGet("assignments")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        [SwaggerOperation(Summary = "List assignments", Description = "Filter by class or student, due date range, with pagination.")]
        public async Task<IActionResult> GetAssignments(
            [FromQuery, SwaggerParameter("Filter by class id")] Guid? classId,
            [FromQuery, SwaggerParameter("Filter by student id")] Guid? studentId,
            [FromQuery, SwaggerParameter("Due date from")] DateTime? dueFrom,
            [FromQuery, SwaggerParameter("Due date to")] DateTime? dueTo,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var list = await _teacherService.GetAssignmentsAsync(classId, studentId, dueFrom, dueTo, page, pageSize, ct);
            return Ok(list);
        }

        [HttpPost("{teacherId:guid}/results/{resultId:guid}/publish")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> PublishResult(Guid teacherId, Guid resultId, [FromQuery] bool publish = true, CancellationToken ct = default)
        {
            var updated = await _teacherService.PublishResultAsync(resultId, publish, ct);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
    }
}
