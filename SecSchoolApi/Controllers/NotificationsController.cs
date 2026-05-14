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
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public NotificationsController(INotificationService notificationService, IMapper mapper)
        {
            _notificationService = notificationService;
            _mapper = mapper;
        }

        [HttpPost("sms")]
        public async Task<IActionResult> SendSms([FromBody] SmsDto dto)
        {
            string phone = dto.Phone;
            string message = dto.Message;
            var ok = await _notificationService.SendSmsAsync(phone, message);
            return ok ? Ok() : BadRequest();
        }

        [HttpPost("email")]
        public async Task<IActionResult> SendEmail([FromBody] EmailDto dto)
        {
            var ok = await _notificationService.SendEmailAsync(dto.Email, dto.Subject, dto.Body);
            return ok ? Ok() : BadRequest();
        }

        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUserNotifications(Guid userId)
        {
            var list = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(_mapper.Map<IEnumerable<Notification>>(list));
        }
    }
}
