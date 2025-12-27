using LearningPlatform.DTOs;
using LearningPlatform.Models;
using LearningPlatform.Repositories;

namespace LearningPlatform.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<string> AddToCartAsync(int userId, int courseId)
        {
            var existingItem = await _cartRepository.GetCartItemAsync(userId, courseId);
            if (existingItem != null)
            {
                return "This course is already in your cart.";
            }

            var newItem = new CartItem
            {
                UserId = userId,
                CourseId = courseId
            };

            await _cartRepository.AddToCartAsync(newItem);
            return "Success";
        }

        public async Task<List<CartItemDto>> GetUserCartAsync(int userId)
        {
            var cartItems = await _cartRepository.GetCartItemsByUserIdAsync(userId);

            return cartItems.Select(item => new CartItemDto
            {
                CartItemId = item.Id,
                CourseId = item.Course.Id,
                Title = item.Course.Title,
                Price = item.Course.Price,
                TeacherName = item.Course.Teacher != null ? item.Course.Teacher.Username : "Unknown"
            }).ToList();
        }
    }
}
