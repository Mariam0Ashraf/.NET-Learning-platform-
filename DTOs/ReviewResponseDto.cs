namespace LearningPlatform.DTOs
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
