using System;
using System.Collections.Generic;

namespace LMS.API.Models.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public string FirstName  { get; set; }
        public string LastName  { get; set; }
        public DateTime DateOfBirth { get; set; }
        
        public ICollection<Literature> Literatures { get; set; }
    }
}