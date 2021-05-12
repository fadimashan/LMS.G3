using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LMS.Core.Entities;
using LMS.Core.Models;

namespace LMS.Data.Data
{
    public class MvcDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public DbSet<Course> Course { get; set; }
        public DbSet<Module> Module { get; set; }
        public DbSet<Activity> Activity { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<AuthorDto> AuthorDto { get; set; }
        //public DbSet<AuthorCreationDto> AuthorCDto { get; set; }

        public MvcDbContext(DbContextOptions<MvcDbContext> options)
          : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUserCourse>().HasKey(a => new { a.CourseId, a.ApplicationUserId });

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Courses)
                .WithMany(c => c.Students)
                .UsingEntity<ApplicationUserCourse>(
                    a => a.HasOne(a => a.Course).WithMany(c => c.Enrollments),
                    a => a.HasOne(app => app.Student).WithMany(s => s.AttendingCourses));

            //builder.Entity<Module>()
            //    .HasMany(m => m.Activities)
            //    .WithOne(m => m.Module)
            //    .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
