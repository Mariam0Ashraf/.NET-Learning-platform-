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
    [Authorize] // Any logged-in user can have a cart
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;
        private readonly IUserRepository _userRepository;

        public CartController(CartService cartService, IUserRepository userRepository)
        {
            _cartService = cartService;
            _userRepository = userRepository;
        }

        [HttpPost("add/{courseId}")] // usage: api/cart/add/5
        public async Task<IActionResult> AddToCart(int courseId)
        {
            // 1. Who is this?
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return Unauthorized();

            // 2. Add
            var result = await _cartService.AddToCartAsync(user.Id, courseId);

            if (result != "Success")
            {
                return BadRequest(result); // Returns "Already in cart"
            }

            return Ok("Course added to cart!");
        }

        [HttpGet("my-cart")]
        public async Task<IActionResult> GetMyCart()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();
            var user = await _userRepository.GetUserByEmailAsync(email);

            var cartItems = await _cartService.GetUserCartAsync(user.Id);

            return Ok(cartItems);
        }
    }
}
