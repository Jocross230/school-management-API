using SecSchoolApi.Model;
namespace SecSchoolApi.Interface
{
    public interface IPaymentService
    {
        Task<FeePayment> InitiatePaymentAsync(FeePayment payment);
        Task<IEnumerable<FeePayment>> GetPaymentHistoryAsync(Guid parentId);
        Task<bool> VerifyPaymentAsync(string reference);
        Task<object> GetPaymentReportAsync();
    }
}
