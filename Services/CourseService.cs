using LearningPlatform.DTOs;
using LearningPlatform.Models;
using LearningPlatform.Repositories;

namespace LearningPlatform.Services
{
    public class CourseService
    {
        private readonly ICourseRepository _courseRepository;
        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }
        public async Task AddCourseAsync(CreateCourseDto request, int teacherId)
        {
            var course = new Models.Course
            {
                Title = request.Title,
                Description = request.Description,
                TeacherId = teacherId,
                Price = request.Price
            };
            await _courseRepository.AddCourseAsync(course);
        }
        public async Task<List<Course>> GetCoursesForTeacherAsync(int teacherId)
        {
            return await _courseRepository.GetCoursesByTeacherIdAsync(teacherId);
        }
    }
}
