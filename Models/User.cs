namespace LearningPlatform.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Student";
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string? VerificationToken { get; set; }
        public DateTime? VerifiedAt { get; set; } 
        public DateTime? VerificationTokenExpiresAt { get; set; }
        public string? PasswordResetToken { get; set; }
    }
}
