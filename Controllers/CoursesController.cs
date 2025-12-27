using LearningPlatform.DTOs;
using LearningPlatform.Repositories;
using LearningPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearningPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly CourseService _courseService;
        private readonly IUserRepository _userRepository;

        public CoursesController(CourseService courseService, IUserRepository userRepository)
        {
            _courseService = courseService;
            _userRepository = userRepository;
        }

        [HttpPost("add")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> AddCourse(CreateCourseDto request)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized("Token does not contain an email.");

            var teacher = await _userRepository.GetUserByEmailAsync(email);
            if (teacher == null) return Unauthorized("User does not exist.");

            await _courseService.AddCourseAsync(request, teacher.Id);
            return Ok("Course added successfully!");
        }

        [HttpGet("my-courses")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMyCourses()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var teacher = await _userRepository.GetUserByEmailAsync(email);
            if (teacher == null) return Unauthorized();

            var response = await _courseService.GetCoursesForTeacherAsync(teacher.Id);

            return Ok(response);
        }

        [HttpGet("all-courses")]
        [Authorize]
        public async Task<IActionResult> GetAllCourses()
        {
            var response = await _courseService.GetAllCoursesAsync();

            return Ok(response);
        }
        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> Search([FromQuery] string q)
        {           
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest("Search term cannot be empty.");
            }
            var response = await _courseService.SearchCoursesAsync(q);
            
            if (response.Count == 0)
            {
                return Ok("No courses found matching that name.");
            }

            return Ok(response);
        }
        [HttpGet("{id}")]
        [Authorize] 
        public async Task<IActionResult> GetCourseDetails(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);

            if (course == null)
            {
                return NotFound($"Course with ID {id} not found.");
            }

            return Ok(course);
        }
    }
}