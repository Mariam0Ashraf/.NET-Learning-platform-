using LearningPlatform.Models;
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Always keep this

            // 1. Fix the "Multiple Cascade Paths" Error
            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction); // <--- STOP CASCADE HERE

            // 2. Fix the "Decimal Price" Warning
            modelBuilder.Entity<Course>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}

