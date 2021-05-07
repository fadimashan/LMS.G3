using System.Collections.Generic;
using LMS.API.Models.Entities;

namespace LMS.API.Models.DTO
{
    public class AuthorDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
  
    }
}
