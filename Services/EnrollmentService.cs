using LearningPlatform.DTOs;
using LearningPlatform.Repositories;

namespace LearningPlatform.Services
{
    public class EnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        // 1. FOR STUDENTS: "My Learning"
        public async Task<List<CourseResponseDto>> GetStudentEnrolledCoursesAsync(int userId)
        {
            var enrollments = await _enrollmentRepository.GetStudentEnrollmentsAsync(userId);

            return enrollments.Select(e => new CourseResponseDto
            {
                Id = e.Course.Id,
                Title = e.Course.Title,
                Description = e.Course.Description,
                Price = e.Course.Price,
                TeacherName = e.Course.Teacher != null ? e.Course.Teacher.Username : "Unknown"
            }).ToList();
        }

        // 2. FOR TEACHERS: "Dashboard"
        public async Task<List<TeacherDashboardDto>> GetTeacherStatsAsync(int teacherId)
        {
            var allEnrollments = await _enrollmentRepository.GetTeacherEnrollmentsAsync(teacherId);

            // Group by Course so the teacher sees stats per course
            var grouped = allEnrollments
                .GroupBy(e => e.Course.Title)
                .Select(g => new TeacherDashboardDto
                {
                    CourseTitle = g.Key,
                    TotalStudents = g.Count(),
                    TotalEarnings = g.Sum(e => e.AmountPaid),
                    Students = g.Select(e => new StudentEnrolledDto
                    {
                        StudentName = e.User.Username,
                        Email = e.User.Email,
                        EnrollDate = e.EnrollDate
                    }).ToList()
                })
                .ToList();

            return grouped;
        }
    }
}
