using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecSchoolApi.Interface;
using SecSchoolApi.Model;
using System.Data;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace SecSchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IParentService _parentService;
        private readonly IMapper _mapper;

        public AdminController(IAdminService adminService, IParentService parentService, IMapper mapper)
        {
            _adminService = adminService;
            _parentService = parentService;
            _mapper = mapper;
        }

        [HttpPost("register-student")]
        public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var student = new StudentModel
            {
                FullName = dto.FullName,
                DateOfBirth = dto.DateOfBirth,
                Class = dto.Class,
                ParentId = dto.ParentId
            };

            try
            {
                var created = await _adminService.RegisterStudentAsync(student);
                return CreatedAtAction(nameof(StudentsController.GetById), "Students", new { id = created.Id }, _mapper.Map<StudentModel>(created));

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost("parents")]
        public async Task<IActionResult> RegisterParent([FromBody] RegisterParentDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var parent = new ParentModel { FullName = dto.FullName, Email = dto.Email };
            var created = await _parentService.CreateAsync(parent, ct);
            return CreatedAtAction(nameof(GetParentById), new { id = created.Id }, created);
        }

        [HttpGet("parents")]
        public async Task<IActionResult> GetParents(CancellationToken ct)
        {
            var parents = await _parentService.GetAllAsync(ct);
            return Ok(parents);
        }

        [HttpGet("parents/{id:guid}")]
        public async Task<IActionResult> GetParentById(Guid id, CancellationToken ct)
        {
            var parent = await _parentService.GetByIdAsync(id, ct);
            if (parent == null) return NotFound();
            return Ok(parent);
        }

        [HttpPut("parents/{id:guid}")]
        public async Task<IActionResult> UpdateParent(Guid id, [FromBody] RegisterParentDto dto, CancellationToken ct)
        {
            var updated = await _parentService.UpdateAsync(id, new ParentModel { FullName = dto.FullName, Email = dto.Email }, ct);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("parents/{id:guid}")]
        public async Task<IActionResult> DeleteParent(Guid id, CancellationToken ct)
        {
            var ok = await _parentService.DeleteAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }




        [HttpPost("Announcements")]
        public async Task<IActionResult> PublishAnnouncement([FromBody] AnnouncementModel dto)
        {
            var a = _mapper.Map<AnnouncementModel>(dto);
            var published = await _adminService.PublishAnnouncementAsync(a);
            return Ok(_mapper.Map<AnnouncementModel>(published));
        }

        [HttpGet("accommodation/requests")]
        [SwaggerOperation(Summary = "List accommodation requests", Description = "Filter by status: Pending/Approved/Rejected.")]
        public async Task<IActionResult> GetAccommodationRequests([FromServices] IAccommodationService accommodation, [FromQuery] AccommodationStatus? status, CancellationToken ct)
        {
            var list = await accommodation.GetRequestsAsync(status, ct);
            return Ok(list);
        }

        [HttpPost("accommodation/requests/{requestId:guid}/allocate")]
        [SwaggerOperation(Summary = "Allocate room to request", Description = "Approves the request and increments room occupancy.")]
        public async Task<IActionResult> AllocateAccommodation([FromServices] IAccommodationService accommodation, Guid requestId, [FromQuery] Guid roomId, CancellationToken ct)
        {
            var updated = await accommodation.AllocateAsync(requestId, roomId, ct);
            if (updated == null) return BadRequest("Cannot allocate.");
            return Ok(updated);
        }

        [HttpPost("accommodation/requests/{requestId:guid}/reject")]
        [SwaggerOperation(Summary = "Reject accommodation request")]
        public async Task<IActionResult> RejectAccommodation([FromServices] IAccommodationService accommodation, Guid requestId, [FromQuery] string? remark, CancellationToken ct)
        {
            var updated = await accommodation.RejectAsync(requestId, remark, ct);
            if (updated == null) return BadRequest("Cannot reject.");
            return Ok(updated);
        }

        [HttpPost("rooms")]
        [SwaggerOperation(Summary = "Create room", Description = "Requires role: Admin.")]
        [SwaggerResponse(200, "Created room", typeof(Room))]
        [SwaggerResponseExample(200, typeof(SecSchoolApi.Swagger.RoomExample))]
        public async Task<IActionResult> CreateRoom([FromServices] IAccommodationService accommodation, [FromBody] CreateRoomDto dto, CancellationToken ct)
        {
            var room = await accommodation.CreateRoomAsync(new Room { Name = dto.Name, Hostel = dto.Hostel, Capacity = dto.Capacity }, ct);
            return Ok(room);
        }

        [HttpGet("rooms")]
        [SwaggerOperation(Summary = "List rooms", Description = "Each room includes Available = Capacity - Occupied.")]
        public async Task<IActionResult> GetRooms([FromServices] IAccommodationService accommodation, CancellationToken ct)
        {
            var rooms = await accommodation.GetRoomsAsync(ct);
            return Ok(rooms);
        }
    }
}
