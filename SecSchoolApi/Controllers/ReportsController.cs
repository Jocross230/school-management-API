using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecSchoolApi.Interface;

namespace SecSchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsService _reports;
        public ReportsController(IReportsService reports) => _reports = reports;

        [HttpGet("students/{studentId:guid}/terms/{termId:guid}/report-card")]
        [Authorize(Roles = "Admin,Teacher,Parent")]
        public async Task<IActionResult> GetReportCard(Guid studentId, Guid termId, CancellationToken ct)
            => Ok(await _reports.GetReportCardAsync(studentId, termId, ct));
    }
}
