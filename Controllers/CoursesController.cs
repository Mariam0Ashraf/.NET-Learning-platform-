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
    public class CoursesController : ControllerBase
    {
        private readonly CourseService _courseService;
        private readonly IUserRepository _userRepository;
        public CoursesController(CourseService courseService,IUserRepository userRepository)
        {
            _courseService = courseService;
            _userRepository = userRepository;

        }
        [HttpPost("add")]
        [Authorize(Roles ="Teacher")]
        public async Task<IActionResult> AddCourse(DTOs.CreateCourseDto request)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("Token does not contain an email.");
            }
            var teacher = await _userRepository.GetUserByEmailAsync(email);

            if (teacher == null)
            {
                return Unauthorized("User does not exist.");
            }


            await _courseService.AddCourseAsync(request, teacher.Id);
            return Ok("Course added successfully!");
        }
    }
}
