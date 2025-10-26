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
    public class BillingController : ControllerBase
    {
        private readonly IBillingService _billing;
        public BillingController(IBillingService billing) => _billing = billing;

        [HttpPost("invoices")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateInvoice([FromBody] Invoice dto, CancellationToken ct)
            => Ok(await _billing.CreateInvoiceAsync(dto, ct));

        [HttpGet("invoices")]
        [Authorize(Roles = "Admin,Parent")] 
        public async Task<IActionResult> GetInvoices([FromQuery] Guid? parentId, [FromQuery] Guid? studentId, [FromQuery] InvoiceStatus? status, CancellationToken ct)
            => Ok(await _billing.GetInvoicesAsync(parentId, studentId, status, ct));

        [HttpPost("invoices/{invoiceId:guid}/pay")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkPaid(Guid invoiceId, CancellationToken ct)
        {
            var inv = await _billing.MarkPaidAsync(invoiceId, ct);
            return inv == null ? NotFound() : Ok(inv);
        }

        [HttpPost("invoices/{invoiceId:guid}/refund")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Refund(Guid invoiceId, CancellationToken ct)
        {
            var inv = await _billing.RefundAsync(invoiceId, ct);
            return inv == null ? NotFound() : Ok(inv);
        }
    }
}
