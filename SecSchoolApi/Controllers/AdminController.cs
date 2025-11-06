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
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;

        public AdminController(IAdminService adminService, IParentService parentService, IStudentService studentService, IMapper mapper)
        {
            _adminService = adminService;
            _parentService = parentService;
            _studentService = studentService;
            _mapper = mapper;
        }

        // Admin management
        [HttpGet("admins")]
        [SwaggerOperation(OperationId = "Admin_List", Summary = "List admins")]
        public async Task<IActionResult> GetAdmins()
        {
            var admins = await _adminService.GetAllAsync();
            return Ok(admins);
        }

        // Student management (admin section)
        [HttpGet("students")]
        [SwaggerOperation(OperationId = "Admin_Students_List", Summary = "List students (paged)")]
        public async Task<IActionResult> ListStudents([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var paged = await _studentService.GetPagedAsync(page, pageSize, ct);
            return Ok(paged);
        }

        [HttpGet("students/{id:guid}")]
        [SwaggerOperation(OperationId = "Admin_Students_GetById", Summary = "Get student by id")]
        public async Task<IActionResult> GetStudentById(Guid id)
        {
            var student = await _studentService.GetByIdAsync(id);
            return student is null ? NotFound() : Ok(student);
        }

        [HttpPost("students")]
        [SwaggerOperation(OperationId = "Admin_Students_Create", Summary = "Register student")]
        public async Task<IActionResult> CreateStudent([FromBody] RegisterStudentDto dto)
        {
            var created = await _studentService.CreateAsync(new StudentModel
            {
                FullName = dto.FullName,
                DateOfBirth = dto.DateOfBirth,
                Class = dto.Class,
                HealthIssue = dto.HealthIssue,
                ParentId = dto.ParentId
            });
            return CreatedAtAction(nameof(GetStudentById), new { id = created.Id }, created);
        }

        [HttpPut("students/{id:guid}")]
        [SwaggerOperation(OperationId = "Admin_Students_Update", Summary = "Update student")]
        public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] StudentModel dto)
        {
            var updated = await _studentService.UpdateAsync(id, new StudentModel { FullName = dto.FullName, Class = dto.Class });
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("students/{id:guid}")]
        [SwaggerOperation(OperationId = "Admin_Students_Delete", Summary = "Delete student")]
        public async Task<IActionResult> DeleteStudent(Guid id)
        {
            var ok = await _studentService.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        [HttpGet("admins/{id:guid}")]
        [SwaggerOperation(OperationId = "Admin_GetById", Summary = "Get admin by id")]
        public async Task<IActionResult> GetAdminById(Guid id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            if (admin == null) return NotFound();
            return Ok(admin);
        }

        [HttpPost("admins")]
        [SwaggerOperation(OperationId = "Admin_Create", Summary = "Create admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterAdminDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _adminService.RegisterAdminAsync(new Admin { FullName = dto.FullName, Email = dto.Email, PhoneNumber = dto.PhoneNumber });
            return CreatedAtAction(nameof(GetAdminById), new { id = created.Id }, created);
        }

        [HttpPut("admins/{id:guid}")]
        [SwaggerOperation(OperationId = "Admin_Update", Summary = "Update admin")]
        public async Task<IActionResult> UpdateAdmin(Guid id, [FromBody] Admin admin)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _adminService.UpdateAsync(id, admin);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("admins/{id:guid}")]
        [SwaggerOperation(OperationId = "Admin_DeleteById", Summary = "Soft-delete admin by id")]
        public async Task<IActionResult> DeleteAdmin(Guid id)
        {
            var ok = await _adminService.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("admins/by-user/{userId:guid}")]
        [SwaggerOperation(OperationId = "Admin_DeleteByUserId", Summary = "Soft-delete admin by user id")]
        public async Task<IActionResult> DeleteAdminByUser(Guid userId)
        {
            var ok = await _adminService.DeleteByUserIdAsync(userId);
            return ok ? NoContent() : NotFound();
        }

        [HttpPost("admins/{id:guid}/restore")]
        [SwaggerOperation(OperationId = "Admin_RestoreById", Summary = "Restore admin by id")]
        public async Task<IActionResult> RestoreAdmin(Guid id)
        {
            var ok = await _adminService.RestoreAsync(id);
            return ok ? Ok() : NotFound();
        }

        [HttpPost("admins/by-user/{userId:guid}/restore")]
        [SwaggerOperation(OperationId = "Admin_RestoreByUserId", Summary = "Restore admin by user id")]
        public async Task<IActionResult> RestoreAdminByUser(Guid userId)
        {
            var ok = await _adminService.RestoreByUserIdAsync(userId);
            return ok ? Ok() : NotFound();
        }

        [HttpPost("parents")]
        [SwaggerOperation(OperationId = "Parent_Create", Summary = "Create parent")]
        public async Task<IActionResult> RegisterParent([FromBody] RegisterParentDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var parent = new ParentModel { FullName = dto.FullName, Email = dto.Email };
            var created = await _parentService.CreateAsync(parent, ct);
            return CreatedAtAction(nameof(GetParentById), new { id = created.Id }, created);
        }

        [HttpGet("parents")]
        [SwaggerOperation(OperationId = "Parent_List", Summary = "List parents")]
        public async Task<IActionResult> GetParents(CancellationToken ct)
        {
            var parents = await _parentService.GetAllAsync(ct);
            return Ok(parents);
        }

        [HttpGet("parents/{id:guid}")]
        [SwaggerOperation(OperationId = "Parent_GetById", Summary = "Get parent by id")]
        public async Task<IActionResult> GetParentById(Guid id, CancellationToken ct)
        {
            var parent = await _parentService.GetByIdAsync(id, ct);
            if (parent == null) return NotFound();
            return Ok(parent);
        }

        [HttpPut("parents/{id:guid}")]
        [SwaggerOperation(OperationId = "Parent_Update", Summary = "Update parent")]
        public async Task<IActionResult> UpdateParent(Guid id, [FromBody] RegisterParentDto dto, CancellationToken ct)
        {
            var updated = await _parentService.UpdateAsync(id, new ParentModel { FullName = dto.FullName, Email = dto.Email }, ct);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("parents/{id:guid}")]
        [SwaggerOperation(OperationId = "Parent_Delete", Summary = "Delete parent")]
        public async Task<IActionResult> DeleteParent(Guid id, CancellationToken ct)
        {
            var ok = await _parentService.DeleteAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }




        [HttpPost("Announcements")]
        [SwaggerOperation(OperationId = "Announcement_Publish", Summary = "Publish announcement")]
        public async Task<IActionResult> PublishAnnouncement([FromBody] CreateAnnouncementDto dto)
        {
            var a = new AnnouncementModel { Title = dto.Title, Message = dto.Message };
            var published = await _adminService.PublishAnnouncementAsync(a);
            return Ok(published);
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
