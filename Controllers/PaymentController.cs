using LearningPlatform.DTOs;
using LearningPlatform.Repositories;
using LearningPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearningPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly IUserRepository _userRepository;

        public PaymentController(PaymentService paymentService, IUserRepository userRepository)
        {
            _paymentService = paymentService;
            _userRepository = userRepository;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] PaymentDto paymentDetails)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();
            var user = await _userRepository.GetUserByEmailAsync(email);

            var result = await _paymentService.ProcessCheckoutAsync(user.Id, paymentDetails);

            if (result != "Success")
            {
                return BadRequest(result);
            }

            return Ok("Payment successful! You have been enrolled in the courses.");
        }
    }
}
