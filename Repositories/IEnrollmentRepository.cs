using LearningPlatform.Models;

namespace LearningPlatform.Repositories
{
    public interface IEnrollmentRepository
    {
        Task AddEnrollmentAsync(Enrollment enrollment);
        Task<bool> IsEnrolledAsync(int userId, int courseId); 
        Task<List<Enrollment>> GetStudentEnrollmentsAsync(int userId);
        Task<List<Enrollment>> GetTeacherEnrollmentsAsync(int teacherId);
    }
}
