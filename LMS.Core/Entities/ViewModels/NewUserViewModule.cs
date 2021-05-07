﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Entities.ViewModels
{
    public class NewUserViewModule
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public RoleType RoleType { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
