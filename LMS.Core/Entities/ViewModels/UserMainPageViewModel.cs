using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Entities.ViewModels
{
    public class UserMainPageViewModel
    {

        public Course Course { get; set; }

        public ICollection<Module> Modules { get; set; }

        public ICollection<Activity> Activities { get; set; }


    }
}
