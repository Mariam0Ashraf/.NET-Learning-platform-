using LearningPlatform.Models;

namespace LearningPlatform.Repositories
{
    public interface ICartRepository
    {
        Task AddToCartAsync(CartItem item);
        Task<List<CartItem>> GetCartItemsByUserIdAsync(int userId);
        Task<CartItem?> GetCartItemAsync(int userId, int courseId); 
    }
}
