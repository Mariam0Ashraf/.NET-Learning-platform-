using LearningPlatform.Data;
using LearningPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;
        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddCourseAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Include(c => c.Teacher) 
                .ToListAsync();
        }
        public async Task<List<Course>> GetCoursesByTeacherIdAsync(int teacherId)
        {
            return await _context.Courses
                .Where(c => c.TeacherId == teacherId) 
                .ToListAsync();
        }
        public async Task<List<Course>> SearchCoursesAsync(string searchTerm)
        {
            return await _context.Courses
                .Include(c => c.Teacher) // Don't forget to include the teacher!
                .Where(c => c.Title.Contains(searchTerm)) // Finds any title containing the text
                .ToListAsync();
        }
    }
}
