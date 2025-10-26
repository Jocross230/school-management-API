using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecSchoolApi.Interface;
using SecSchoolApi.Model;
using System.Data;

namespace SecSchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SchoolsController : ControllerBase
    {
        private readonly ISchoolService _schoolService;
        private readonly IMapper _mapper;

        public SchoolsController(ISchoolService schoolService, IMapper mapper)
        {
            _schoolService = schoolService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(_mapper.Map<IEnumerable<School>>(await _schoolService.GetAllAsync()));

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] School dto)
        {
            var s = _mapper.Map<School>(dto);
            var created = await _schoolService.RegisterAsync(s);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, _mapper.Map<School>(created));
        }

        [HttpGet("{id:guid}/branding")]
        public async Task<IActionResult> GetBranding(Guid id)
        {
            var branding = await _schoolService.GetBrandingAsync(id);
            if (branding == null) return NotFound();
            return Ok(_mapper.Map<Branding>(branding));
        }

        [HttpPost("{id:guid}/branding")]
        public async Task<IActionResult> SetBranding(Guid id, [FromBody] Branding dto, CancellationToken ct)
        {
            var saved = await _schoolService.SetBrandingAsync(id, dto, ct);
            return Ok(saved);
        }
    }
}
