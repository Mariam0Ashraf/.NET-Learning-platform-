using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.DTOs
{
    public class CreateReviewDto
    {
        public int CourseId { get; set; }

        [Required]
        public string Content { get; set; }

        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
        public int Rating { get; set; }
    }
}
