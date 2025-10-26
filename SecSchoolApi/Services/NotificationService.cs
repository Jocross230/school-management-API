using SecSchoolApi.Interface;
using SecSchoolApi.Model;
using SecSchoolApi.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace SecSchoolApi.Services
{
    public class NotificationService : INotificationService
    {
        private readonly SchoolDbContext _db;
        public NotificationService(SchoolDbContext db) => _db = db;

        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            var n = new Notification { UserId = Guid.Empty, Channel = "sms", Message = $"To:{phoneNumber} {message}", IsDelivered = true, CreatedAt = DateTime.UtcNow };
            _db.Notifications.Add(n);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SendEmailAsync(string email, string subject, string body)
        {
            var n = new Notification { UserId = Guid.Empty, Channel = "email", Message = $"To:{email} Subject:{subject} Body:{body}", IsDelivered = true, CreatedAt = DateTime.UtcNow };
            _db.Notifications.Add(n);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SendPushAsync(Guid userId, string message)
        {
            var n = new Notification { UserId = userId, Channel = "push", Message = message, IsDelivered = true, CreatedAt = DateTime.UtcNow };
            _db.Notifications.Add(n);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId) =>
            await _db.Notifications.Where(n => n.UserId == userId).OrderByDescending(n => n.CreatedAt).ToListAsync();
    }
}
