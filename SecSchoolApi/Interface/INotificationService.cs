using SecSchoolApi.Model;

namespace SecSchoolApi.Interface
{
    public interface INotificationService
    {
        Task<bool> SendSmsAsync(string phoneNumber, string message);
        Task<bool> SendEmailAsync(string email, string subject, string body);
        Task<bool> SendPushAsync(Guid userId, string message);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId);
    }
}
