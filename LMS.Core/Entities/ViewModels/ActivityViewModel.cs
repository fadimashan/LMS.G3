using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Entities.ViewModels
{
    public class ActivityViewModel
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public ActivityType ActivityType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Description { get; set; }

        public Course Course { get; set; }

        public Module Module { get; set; }


    }
}
