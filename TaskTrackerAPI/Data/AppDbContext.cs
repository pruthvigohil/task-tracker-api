using Microsoft.EntityFrameworkCore;
using TaskTrackerAPI.Entities;

namespace TaskTrackerAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<Comment> Comments => Set<Comment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User config
            modelBuilder.Entity<User>(e =>
            {
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.Role).HasDefaultValue("Employee");
            });

            // TaskItem config
            modelBuilder.Entity<TaskItem>(e =>
            {
                e.HasIndex(t => t.Title).IsUnique();
                e.HasOne(t => t.AssignedUser)
                 .WithMany(u => u.AssignedTasks)
                 .HasForeignKey(t => t.AssignedUserId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // Comment config
            modelBuilder.Entity<Comment>(e =>
            {
                e.HasOne(c => c.Task)
                 .WithMany(t => t.Comments)
                 .HasForeignKey(c => c.TaskId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(c => c.User)
                 .WithMany(u => u.Comments)
                 .HasForeignKey(c => c.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
