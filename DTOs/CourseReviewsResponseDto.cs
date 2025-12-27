namespace LearningPlatform.DTOs
{
    public class CourseReviewsResponseDto
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }    
        public List<ReviewResponseDto> Reviews { get; set; } 
    }
}
