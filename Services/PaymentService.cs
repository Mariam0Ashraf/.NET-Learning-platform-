using LearningPlatform.DTOs;
using LearningPlatform.Models;
using LearningPlatform.Repositories;
using Stripe; // Import the library

public class PaymentService
{
    private readonly ICartRepository _cartRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IConfiguration _configuration; // To read the API Key

    public PaymentService(ICartRepository cartRepo, IEnrollmentRepository enrollmentRepo, IConfiguration config)
    {
        _cartRepository = cartRepo;
        _enrollmentRepository = enrollmentRepo;
        _configuration = config;
    }

    public async Task<string> ProcessCheckoutAsync(int userId, PaymentDto paymentDetails)
    {
        // 1. Get Cart
        var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(userId);
        if (cartItems.Count == 0) return "Cart is empty.";

        // 2. Calculate Total (Stripe uses "Cents", so $10.00 = 1000 cents)
        var totalAmount = cartItems.Sum(c => c.Course.Price);
        var amountInCents = (long)(totalAmount * 100);

        try
        {
            // 3. TALK TO STRIPE (Real API Call)
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

            var options = new ChargeCreateOptions
            {
                Amount = amountInCents,
                Currency = "usd",
                Source = paymentDetails.PaymentToken, // e.g., "tok_visa"
                Description = $"Purchase by User ID {userId}"
            };

            var service = new ChargeService();
            Charge charge = await service.CreateAsync(options); // <--- The Actual Charge

            if (charge.Status != "succeeded")
            {
                return "Payment failed at Stripe.";
            }
        }
        catch (StripeException ex)
        {
            return $"Stripe Error: {ex.Message}";
        }

        // 4. Success! Create Enrollments
        foreach (var item in cartItems)
        {
            bool alreadyOwned = await _enrollmentRepository.IsEnrolledAsync(userId, item.CourseId);
            if (!alreadyOwned)
            {
                var enrollment = new Enrollment
                {
                    UserId = userId,
                    CourseId = item.CourseId,
                    AmountPaid = item.Course.Price,
                    EnrollDate = DateTime.Now
                };
                await _enrollmentRepository.AddEnrollmentAsync(enrollment);
            }
        }

        // 5. Clear Cart
        await _cartRepository.ClearCartAsync(userId);

        return "Success";
    }
}