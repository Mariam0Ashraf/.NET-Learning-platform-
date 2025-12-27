using LearningPlatform.DTOs;
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
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _reviewService;
        private readonly IUserRepository _userRepository;

        public ReviewsController(ReviewService reviewService, IUserRepository userRepository)
        {
            _reviewService = reviewService;
            _userRepository = userRepository;
        }

        [HttpPost("add")]
        [Authorize] 
        public async Task<IActionResult> AddReview(CreateReviewDto request)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return Unauthorized();
            var user = await _userRepository.GetUserByEmailAsync(email);

            await _reviewService.AddReviewAsync(request, user.Id);

            return Ok("Review added successfully!");
        }

        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetReviews(int courseId)
        {
            var result = await _reviewService.GetCourseReviewsAsync(courseId);

            return Ok(result);
        }
    }
}
