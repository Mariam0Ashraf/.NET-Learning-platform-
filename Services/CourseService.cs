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
            var course = new Course
            {
                Title = request.Title,
                Description = request.Description,
                TeacherId = teacherId,
                Price = request.Price
            };
            await _courseRepository.AddCourseAsync(course);
        }

        public async Task<List<CourseResponseDto>> GetCoursesForTeacherAsync(int teacherId)
        {
            var courses = await _courseRepository.GetCoursesByTeacherIdAsync(teacherId);

            return courses.Select(c => new CourseResponseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Price = c.Price,
                TeacherName = c.Teacher != null ? c.Teacher.Username : "Unknown"
            }).ToList();
        }

        public async Task<List<CourseResponseDto>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.GetAllCoursesAsync();

            return courses.Select(c => new CourseResponseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                Price = c.Price,
                TeacherName = c.Teacher != null ? c.Teacher.Username : "Unknown"
            }).ToList();
        }
    }
}