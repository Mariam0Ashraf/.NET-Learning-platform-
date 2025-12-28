using LearningPlatform.DTOs;
using LearningPlatform.Models;
using LearningPlatform.Repositories;
using Stripe; 

public class PaymentService
{
    private readonly ICartRepository _cartRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IConfiguration _configuration; 

    public PaymentService(ICartRepository cartRepo, IEnrollmentRepository enrollmentRepo, IConfiguration config)
    {
        _cartRepository = cartRepo;
        _enrollmentRepository = enrollmentRepo;
        _configuration = config;
    }

    public async Task<string> ProcessCheckoutAsync(int userId, PaymentDto paymentDetails)
    {
       
        var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(userId);
        if (cartItems.Count == 0) return "Cart is empty.";

        
        var totalAmount = cartItems.Sum(c => c.Course.Price);
        var amountInCents = (long)(totalAmount * 100);

        try
        {
          
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

            var options = new ChargeCreateOptions
            {
                Amount = amountInCents,
                Currency = "usd",
                Source = paymentDetails.PaymentToken, 
                Description = $"Purchase by User ID {userId}"
            };

            var service = new ChargeService();
            Charge charge = await service.CreateAsync(options);

            if (charge.Status != "succeeded")
            {
                return "Payment failed at Stripe.";
            }
        }
        catch (StripeException ex)
        {
            return $"Stripe Error: {ex.Message}";
        }

       
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

        await _cartRepository.ClearCartAsync(userId);

        return "Success";
    }
}