using LearningPlatform.Data;
using LearningPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly AppDbContext _context;
        public EnrollmentRepository(AppDbContext context) { _context = context; }

        public async Task AddEnrollmentAsync(Enrollment enrollment)
        {
            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsEnrolledAsync(int userId, int courseId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
        }
        public async Task<List<Enrollment>> GetStudentEnrollmentsAsync(int userId)
{
            return await _context.Enrollments
                .Where(e => e.UserId == userId)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Teacher) // Load Teacher name for the student view
                .ToListAsync();
}

        public async Task<List<Enrollment>> GetTeacherEnrollmentsAsync(int teacherId)
        {
            // Find all enrollments where the Course's TeacherId matches
            return await _context.Enrollments
                .Where(e => e.Course.TeacherId == teacherId)
                .Include(e => e.Course) // Load Course Info
                .Include(e => e.User)   // Load Student Info
                .ToListAsync();
}
    }
}
