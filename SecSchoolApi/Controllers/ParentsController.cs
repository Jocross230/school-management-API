using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecSchoolApi.Interface;
using SecSchoolApi.Model;

namespace SecSchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ParentsController : ControllerBase
    {
        private readonly IParentService _parentService;
        private readonly IMapper _mapper;

        public ParentsController(IParentService parentService, IMapper mapper)
        {
            _parentService = parentService;
            _mapper = mapper;
        }

        [HttpGet("{parentId:guid}/children")]
        public async Task<IActionResult> GetChildren(Guid parentId)
        {
            var children = await _parentService.GetChildrenAsync(parentId);
            return Ok(_mapper.Map<IEnumerable<StudentModel>>(children));
        }

        [HttpGet("{parentId:guid}/payments/history")]
        public async Task<IActionResult> GetPayments(Guid parentId)
        {
            var payments = await _parentService.GetPaymentHistoryAsync(parentId);
            return Ok(_mapper.Map<IEnumerable<FeePayment>>(payments));
        }

        [HttpPost("{parentId:guid}/messages")]
        public async Task<IActionResult> SendMessage(Guid parentId, [FromBody] Message dto)
        {
            var msg = _mapper.Map<Message>(dto);
            var sent = await _parentService.SendMessageAsync(parentId, msg);
            return Ok(_mapper.Map<Message>(sent));
        }
    }
}
