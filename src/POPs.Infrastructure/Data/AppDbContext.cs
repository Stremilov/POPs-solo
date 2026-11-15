using Microsoft.EntityFrameworkCore;
using POPs.Shared.Models;

namespace POPs.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Administrator> Administrators => Set<Administrator>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Course> Courses => Set<Course>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Group)
            .WithMany(g => g.Students)
            .HasForeignKey(s => s.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Group>()
            .HasOne(g => g.Course)
            .WithMany(c => c.Groups)
            .HasForeignKey(g => g.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Student>()
            .HasIndex(s => s.Login)
            .IsUnique();

        modelBuilder.Entity<Administrator>()
            .HasIndex(a => a.Login)
            .IsUnique();
    }
}
