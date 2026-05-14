using SecSchoolApi.Model;

namespace SecSchoolApi.Interface
{
    public interface IAccommodationService
    {
        Task<AccommodationRequest> RequestAsync(Guid studentId, string? remark, CancellationToken ct = default);
        Task<IEnumerable<AccommodationRequest>> GetStudentRequestsAsync(Guid studentId, CancellationToken ct = default);
        Task<IEnumerable<AccommodationRequest>> GetRequestsAsync(AccommodationStatus? status, CancellationToken ct = default);
        Task<AccommodationRequest?> AllocateAsync(Guid requestId, Guid roomId, CancellationToken ct = default);
        Task<AccommodationRequest?> RejectAsync(Guid requestId, string? remark, CancellationToken ct = default);

        Task<Room> CreateRoomAsync(Room room, CancellationToken ct = default);
        Task<IEnumerable<Room>> GetRoomsAsync(CancellationToken ct = default);
    }
}