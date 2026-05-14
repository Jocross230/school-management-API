using SecSchoolApi.Model;

namespace SecSchoolApi.Interface
{
    public interface IParentService
    {
        Task<ParentModel> CreateAsync(ParentModel parent, CancellationToken ct = default);
        Task<IEnumerable<ParentModel>> GetAllAsync(CancellationToken ct = default);
        Task<ParentModel?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<ParentModel?> UpdateAsync(Guid id, ParentModel parent, CancellationToken ct = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);

        Task<IEnumerable<StudentModel>> GetChildrenAsync(Guid parentId, CancellationToken ct = default);
        Task<IEnumerable<AnnouncementModel>> GetNotificationsAsync(Guid parentId, CancellationToken ct = default);
        Task<IEnumerable<FeePayment>> GetPaymentHistoryAsync(Guid parentId, CancellationToken ct = default);
        Task<Message> SendMessageAsync(Guid parentId, Message message, CancellationToken ct = default);
        Task<IEnumerable<Message>> GetMessagesAsync(Guid parentId, CancellationToken ct = default);
    }
}
