using LearningPlatform.DTOs;
using LearningPlatform.Models.LearningPlatform.Models;
using LearningPlatform.Repositories;

namespace LearningPlatform.Services
{
    public class ReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task AddReviewAsync(CreateReviewDto request, int userId)
        {
            var review = new Review
            {
                CourseId = request.CourseId,
                Content = request.Content,
                Rating = request.Rating,
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            await _reviewRepository.AddReviewAsync(review);
        }

        public async Task<CourseReviewsResponseDto> GetCourseReviewsAsync(int courseId)
        {
            var reviews = await _reviewRepository.GetReviewsByCourseIdAsync(courseId);

            var reviewDtos = reviews.Select(r => new ReviewResponseDto
            {
                Id = r.Id,
                StudentName = r.User.Username,
                Content = r.Content,
                Rating = r.Rating,
                CreatedAt = r.CreatedAt
            }).ToList();

            double avg = 0;
            if (reviewDtos.Count > 0)
            {
                avg = reviewDtos.Average(r => r.Rating); 
            }

           
            return new CourseReviewsResponseDto
            {
                AverageRating = Math.Round(avg, 1), 
                TotalReviews = reviewDtos.Count,
                Reviews = reviewDtos
            };
        }

    }
}
