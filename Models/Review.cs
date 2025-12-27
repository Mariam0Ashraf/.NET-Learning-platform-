namespace LearningPlatform.Models
{
    using System.ComponentModel.DataAnnotations;

    namespace LearningPlatform.Models
    {
        public class Review
        {
            public int Id { get; set; }

            public string Content { get; set; } 

            [Range(0, 5)]
            public int Rating { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.Now;

            public int UserId { get; set; }
            public User User { get; set; }

            public int CourseId { get; set; }
            public Course Course { get; set; }
        }
    }
}
