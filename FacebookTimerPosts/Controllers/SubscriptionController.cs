using FacebookTimerPosts.DTOs;
using FacebookTimerPosts.Enums;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FacebookTimerPosts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly IPaymentResultRepository _paymentResultRepository;
        private readonly ILogger<SubscriptionController> _logger;

        public SubscriptionController(
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IUserSubscriptionRepository userSubscriptionRepository,
            IPaymentResultRepository paymentResultRepository,
            ILogger<SubscriptionController> logger)
        {
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _paymentResultRepository = paymentResultRepository;
            _logger = logger;
        }

        [HttpGet("plans")]
        public async Task<IActionResult> GetSubscriptionPlans()
        {
            var plans = await _subscriptionPlanRepository.GetActiveSubscriptionPlansAsync();

            var plansDto = new List<SubscriptionPlanDto>();
            foreach (var plan in plans)
            {
                plansDto.Add(new SubscriptionPlanDto
                {
                    Id = plan.Id,
                    Name = plan.Name,
                    Description = plan.Description,
                    Price = plan.Price,
                    DurationInDays = plan.DurationInDays,
                    MaxPosts = plan.MaxPosts,
                    MaxTemplates = plan.MaxTemplates,
                    IsActive = plan.IsActive
                });
            }

            return Ok(plansDto);
        }

        [Authorize]
        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentSubscription()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentSubscription = await _userSubscriptionRepository.GetCurrentSubscriptionAsync(userId);

            if (currentSubscription == null)
            {
                return Ok(null);
            }

            int daysRemaining = (int)(currentSubscription.EndDate - DateTime.UtcNow).TotalDays;

            var subscriptionDto = new UserSubscriptionDto
            {
                Id = currentSubscription.Id,
                SubscriptionPlanId = currentSubscription.SubscriptionPlanId,
                SubscriptionPlanName = currentSubscription.SubscriptionPlan.Name,
                StartDate = currentSubscription.StartDate,
                EndDate = currentSubscription.EndDate,
                IsActive = currentSubscription.IsActive,
                AutoRenew = currentSubscription.AutoRenew,
                DaysRemaining = daysRemaining > 0 ? daysRemaining : 0
            };

            return Ok(subscriptionDto);
        }

        [Authorize]
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeDto subscribeDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Validate subscription plan
            if (!await _subscriptionPlanRepository.IsValidSubscriptionPlanIdAsync(subscribeDto.SubscriptionPlanId))
            {
                return BadRequest("Invalid subscription plan");
            }

            // In a real application, you would process the payment here
            // This is a simplified example where we just create the subscription
            try
            {
                // Simulate payment processing
                var subscriptionPlan = await _subscriptionPlanRepository.GetByIdAsync(subscribeDto.SubscriptionPlanId);
                string fakeTransactionId = "payment_" + Guid.NewGuid().ToString();

                // Record payment result
                var paymentResult = await _paymentResultRepository.CreatePaymentResultAsync(
                    userId,
                    null, // Will be updated after subscription creation
                    "Stripe",
                    fakeTransactionId,
                    subscriptionPlan.Price,
                    "USD",
                    PaymentStatus.Completed);

                // Create subscription
                var subscription = await _userSubscriptionRepository.AddSubscriptionAsync(
                    userId,
                    subscribeDto.SubscriptionPlanId,
                    subscribeDto.AutoRenew,
                    fakeTransactionId);

                // Update payment result with subscription ID
                paymentResult.UserSubscriptionId = subscription.Id;
                await _paymentResultRepository.UpdateAsync(paymentResult);

                int daysRemaining = (int)(subscription.EndDate - DateTime.UtcNow).TotalDays;

                var subscriptionDto = new UserSubscriptionDto
                {
                    Id = subscription.Id,
                    SubscriptionPlanId = subscription.SubscriptionPlanId,
                    SubscriptionPlanName = subscriptionPlan.Name,
                    StartDate = subscription.StartDate,
                    EndDate = subscription.EndDate,
                    IsActive = subscription.IsActive,
                    AutoRenew = subscription.AutoRenew,
                    DaysRemaining = daysRemaining > 0 ? daysRemaining : 0
                };

                return Ok(subscriptionDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing subscription for user {UserId}", userId);
                return StatusCode(500, "Failed to process subscription");
            }
        }

        [Authorize]
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelSubscription()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentSubscription = await _userSubscriptionRepository.GetCurrentSubscriptionAsync(userId);

            if (currentSubscription == null)
            {
                return BadRequest("No active subscription found");
            }

            // In a real application, you would cancel the subscription with the payment provider
            // This is a simplified example where we just update the auto-renew flag
            currentSubscription.AutoRenew = false;
            currentSubscription.UpdatedAt = DateTime.UtcNow;

            await _userSubscriptionRepository.UpdateAsync(currentSubscription);

            return Ok(new { message = "Subscription auto-renew canceled successfully" });
        }

        [Authorize]
        [HttpGet("check-renewal-alert")]
        public async Task<IActionResult> CheckRenewalAlert()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var needsAlert = await _userSubscriptionRepository.NeedsSubscriptionRenewalAlertAsync(userId);

            return Ok(new { needsRenewalAlert = needsAlert });
        }

        [Authorize]
        [HttpGet("payments")]
        public async Task<IActionResult> GetPaymentHistory()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var payments = await _paymentResultRepository.GetUserPaymentsAsync(userId);

            var paymentsDto = new List<PaymentResultDto>();
            foreach (var payment in payments)
            {
                paymentsDto.Add(new PaymentResultDto
                {
                    Id = payment.Id,
                    UserSubscriptionId = payment.UserSubscriptionId,
                    PaymentProvider = payment.PaymentProvider,
                    TransactionId = payment.TransactionId,
                    Amount = payment.Amount,
                    Currency = payment.Currency,
                    Status = payment.Status,
                    ErrorMessage = payment.ErrorMessage,
                    CreatedAt = payment.CreatedAt
                });
            }

            return Ok(paymentsDto);
        }
    }
}
