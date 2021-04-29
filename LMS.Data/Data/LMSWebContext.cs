using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LMS.Data.Data
{
    public class LMSWebContext : DbContext
    {
        public LMSWebContext (DbContextOptions<LMSWebContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Course { get; set; }
    }
}
