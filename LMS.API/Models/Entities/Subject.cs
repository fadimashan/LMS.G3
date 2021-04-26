using System.Collections.Generic;

namespace LMS.API.Models.Entities
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public ICollection<Literature> Literatures { get; set; }
    }
}