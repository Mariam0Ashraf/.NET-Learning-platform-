using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.DTOs
{
    public class CreateCourseDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
