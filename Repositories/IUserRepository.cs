using LearningPlatform.Models;

namespace LearningPlatform.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);

    }
}
