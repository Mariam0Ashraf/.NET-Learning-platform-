using LearningPlatform.Models.LearningPlatform.Models;

namespace LearningPlatform.Repositories
{
    public interface IReviewRepository
    {
        Task AddReviewAsync(Review review);
        Task<List<Review>> GetReviewsByCourseIdAsync(int courseId);
    }
}
