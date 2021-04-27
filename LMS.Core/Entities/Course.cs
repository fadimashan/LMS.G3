using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Entities
{
    public class Course
    {

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        // Navigation Properties

        public ICollection<ApplicationUser> ApplicationUsers { get; set; }
        public ICollection<Module> Modules { get; set; }
        public ICollection<Document> Documents { get; set; }

    }
}
