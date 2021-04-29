using System.Collections.Generic;

namespace LMS.API.Models.Entities
{
    public class PublicationType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public ICollection<Publication> Publications { get; set; }
    }
}
