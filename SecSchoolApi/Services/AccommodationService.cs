using SecSchoolApi.Data;
using SecSchoolApi.Interface;
using SecSchoolApi.Model;
using Microsoft.EntityFrameworkCore;

namespace SecSchoolApi.Services
{
    public class AccommodationService : IAccommodationService
    {
        private readonly SchoolDbContext _db;
        private readonly INotificationService _notifications;
        public AccommodationService(SchoolDbContext db, INotificationService notifications)
        {
            _db = db;
            _notifications = notifications;
        }

        public async Task<AccommodationRequest> RequestAsync(Guid studentId, string? remark, CancellationToken ct = default)
        {
            var req = new AccommodationRequest { StudentId = studentId, Remark = remark, Status = AccommodationStatus.Pending };
            _db.AccommodationRequests.Add(req);
            await _db.SaveChangesAsync(ct);
            return req;
        }

        public async Task<IEnumerable<AccommodationRequest>> GetStudentRequestsAsync(Guid studentId, CancellationToken ct = default) =>
            await _db.AccommodationRequests.Where(r => r.StudentId == studentId)
                                           .OrderByDescending(r => r.RequestedAt)
                                           .ToListAsync(ct);

        public async Task<IEnumerable<AccommodationRequest>> GetRequestsAsync(AccommodationStatus? status, CancellationToken ct = default)
        {
            var q = _db.AccommodationRequests.AsQueryable();
            if (status.HasValue) q = q.Where(r => r.Status == status.Value);
            return await q.OrderBy(r => r.Status).ThenByDescending(r => r.RequestedAt).ToListAsync(ct);
        }

        public async Task<AccommodationRequest?> AllocateAsync(Guid requestId, Guid roomId, CancellationToken ct = default)
        {
            var req = await _db.AccommodationRequests.FindAsync(new object[] { requestId }, ct);
            if (req == null || req.Status != AccommodationStatus.Pending) return null;
            var room = await _db.Rooms.FindAsync(new object[] { roomId }, ct);
            if (room == null) return null;

            for (int attempt = 0; attempt < 3; attempt++)
            {
                if (room.Occupied >= room.Capacity) return null;
                room.Occupied += 1;
                req.RoomId = roomId;
                req.Status = AccommodationStatus.Approved;
                req.ProcessedAt = DateTime.UtcNow;
                try
                {
                    await _db.SaveChangesAsync(ct);
                    await _notifications.SendPushAsync(Guid.Empty, $"Accommodation approved for student {req.StudentId} in room {room.Name}");
                    return req;
                }
                catch (DbUpdateConcurrencyException)
                {
                    _db.Entry(room).Reload();
                }
            }
            return null;
        }

        public async Task<AccommodationRequest?> RejectAsync(Guid requestId, string? remark, CancellationToken ct = default)
        {
            var req = await _db.AccommodationRequests.FindAsync(new object[] { requestId }, ct);
            if (req == null || req.Status != AccommodationStatus.Pending) return null;
            req.Status = AccommodationStatus.Rejected;
            req.Remark = remark;
            req.ProcessedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            await _notifications.SendPushAsync(Guid.Empty, $"Accommodation request rejected for student {req.StudentId}. {remark}");
            return req;
        }

        public async Task<Room> CreateRoomAsync(Room room, CancellationToken ct = default)
        {
            _db.Rooms.Add(room);
            await _db.SaveChangesAsync(ct);
            return room;
        }

        public async Task<IEnumerable<Room>> GetRoomsAsync(CancellationToken ct = default) =>
            await _db.Rooms.OrderBy(r => r.Hostel).ThenBy(r => r.Name).ToListAsync(ct);
    }
}
