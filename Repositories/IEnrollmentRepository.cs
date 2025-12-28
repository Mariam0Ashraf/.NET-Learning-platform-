using LearningPlatform.Models;

namespace LearningPlatform.Repositories
{
    public interface IEnrollmentRepository
    {
        Task AddEnrollmentAsync(Enrollment enrollment);
        Task<bool> IsEnrolledAsync(int userId, int courseId); 
    }
}
