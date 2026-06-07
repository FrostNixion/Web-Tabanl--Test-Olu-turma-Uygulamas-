using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestCreationSystem.Models;

namespace TestCreationSystem.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Test> Tests { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<TestResult> TestResults { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Test entity
        builder.Entity<Test>(entity =>
        {
            entity.HasOne(t => t.CreatedByTeacher)
                  .WithMany()
                  .HasForeignKey(t => t.CreatedByTeacherId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Question entity
        builder.Entity<Question>(entity =>
        {
            entity.HasOne(q => q.Test)
                  .WithMany(t => t.Questions)
                  .HasForeignKey(q => q.TestId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Option entity
        builder.Entity<Option>(entity =>
        {
            entity.HasOne(o => o.Question)
                  .WithMany(q => q.Options)
                  .HasForeignKey(o => o.QuestionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure TestResult entity
        builder.Entity<TestResult>(entity =>
        {
            entity.HasOne(tr => tr.Student)
                  .WithMany()
                  .HasForeignKey(tr => tr.StudentId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(tr => tr.Test)
                  .WithMany(t => t.TestResults)
                  .HasForeignKey(tr => tr.TestId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
