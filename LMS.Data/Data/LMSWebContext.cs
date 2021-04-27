using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMS.Data.Data
{
    public class LMSWebContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public LMSWebContext(DbContextOptions<LMSWebContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Course { get; set; }
        public DbSet<ApplicationUserCourse> AppUserCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUserCourse>().HasKey(a => new { a.ApplicationUserId, a.CourseId });

        }
    }
}
