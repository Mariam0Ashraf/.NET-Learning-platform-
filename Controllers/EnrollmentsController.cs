using LearningPlatform.Repositories;
using LearningPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearningPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly EnrollmentService _enrollmentService;
        private readonly IUserRepository _userRepository;

        public EnrollmentsController(EnrollmentService enrollmentService, IUserRepository userRepository)
        {
            _enrollmentService = enrollmentService;
            _userRepository = userRepository;
        }

        // 1. STUDENT: View my purchased courses
        [HttpGet("my-learning")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyLearning()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();
            var student = await _userRepository.GetUserByEmailAsync(email);

            var courses = await _enrollmentService.GetStudentEnrolledCoursesAsync(student.Id);
            return Ok(courses);
        }

        // 2. TEACHER: View who bought my courses
        [HttpGet("teacher-dashboard")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetTeacherDashboard()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();
            var teacher = await _userRepository.GetUserByEmailAsync(email);

            var stats = await _enrollmentService.GetTeacherStatsAsync(teacher.Id);
            return Ok(stats);
        }
    }
}
