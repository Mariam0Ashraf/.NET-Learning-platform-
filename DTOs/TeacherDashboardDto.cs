namespace LearningPlatform.DTOs
{
    public class TeacherDashboardDto
    {
        public string CourseTitle { get; set; }
        public int TotalStudents { get; set; }
        public decimal TotalEarnings { get; set; }
        public List<StudentEnrolledDto> Students { get; set; }
    }
}
