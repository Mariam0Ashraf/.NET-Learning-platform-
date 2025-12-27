namespace LearningPlatform.DTOs
{
    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string TeacherName { get; set; }
    }
}
