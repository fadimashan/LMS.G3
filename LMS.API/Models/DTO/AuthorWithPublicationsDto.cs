using System.Collections.Generic;

namespace LMS.API.Models.DTO
{
    public class AuthorWithPublicationsDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        
        public ICollection<PublicationDto> Publications { get; set; }
    }
}
