using FacebookTimerPosts.AppDbContext;
using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using FacebookTimerPosts.Services.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace FacebookTimerPosts.Services.Repository
{
    public class PaymentResultRepository : Repository<PaymentResult>, IPaymentResultRepository
    {
        private readonly ApplicationDbContext _db;

        public PaymentResultRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<PaymentResult> CreatePaymentResultAsync(string userId, int? userSubscriptionId,
            string paymentProvider, string transactionId, decimal amount, string currency, PaymentStatus status)
        {
            var paymentResult = new PaymentResult
            {
                UserId = userId,
                UserSubscriptionId = userSubscriptionId,
                PaymentProvider = paymentProvider,
                TransactionId = transactionId,
                Amount = amount,
                Currency = currency,
                Status = status,
                CreatedAt = DateTime.UtcNow
            };

            await _db.PaymentResults.AddAsync(paymentResult);
            await _db.SaveChangesAsync();

            return paymentResult;
        }

        public async Task<IList<PaymentResult>> GetUserPaymentsAsync(string userId)
        {
            return await _db.PaymentResults
                .Where(pr => pr.UserId == userId)
                .OrderByDescending(pr => pr.CreatedAt)
                .ToListAsync();
        }
    }
}
