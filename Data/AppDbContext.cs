using LearningPlatform.Models;
using LearningPlatform.Models.LearningPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Stops the cycle

            // Fix the decimal warning for AmountPaid
            modelBuilder.Entity<Enrollment>()
                .Property(e => e.AmountPaid)
                .HasColumnType("decimal(18,2)");
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Course>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}

