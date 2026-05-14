using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecSchoolApi.Interface;
using SecSchoolApi.Model;

namespace SecSchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;

        public PaymentsController(IPaymentService paymentService, IMapper mapper)
        {
            _paymentService = paymentService;
            _mapper = mapper;
        }

        [HttpPost("fees")]
        public async Task<IActionResult> Initiate([FromBody] FeePayment dto)
        {
            var model = _mapper.Map<FeePayment>(dto);
            var created = await _paymentService.InitiatePaymentAsync(model);
            return Ok(_mapper.Map<FeePayment>(created));
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromQuery] string reference)
        {
            var ok = await _paymentService.VerifyPaymentAsync(reference);
            return ok ? Ok() : NotFound();
        }

        [HttpGet("history/{parentId:guid}")]
        public async Task<IActionResult> History(Guid parentId)
        {
            var list = await _paymentService.GetPaymentHistoryAsync(parentId);
            return Ok(_mapper.Map<IEnumerable<FeePayment>>(list));
        }
    }
}
