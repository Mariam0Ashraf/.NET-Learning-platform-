using LearningPlatform.Models;

namespace LearningPlatform.Repositories
{
    public interface ICourseRepository
    {
        Task AddCourseAsync(Course course);
        Task<List<Course?>> GetAllCoursesAsync();
        Task<List<Course>> GetCoursesByTeacherIdAsync(int teacherId);
        Task<List<Course>> SearchCoursesAsync(string searchTerm);
    }
}
