namespace LearningPlatform.Models
{
    public class Course
    {
        public int Id { set; get; }
        public string Title { set; get; } = string.Empty;
        public string Description { set; get; } = string.Empty;
        public decimal Price { set; get; }
        public int TeacherId { set; get; }
        public User Teacher { set; get; }
    }
}
