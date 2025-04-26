using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository.Base;

namespace FacebookTimerPosts.Services.IRepository
{
    public interface IPaymentResultRepository : IRepository<PaymentResult>
    {
        Task<IList<PaymentResult>> GetUserPaymentsAsync(string userId);
        Task<PaymentResult> CreatePaymentResultAsync(string userId, int? userSubscriptionId,
            string paymentProvider, string transactionId, decimal amount, string currency, PaymentStatus status);
    }
}
