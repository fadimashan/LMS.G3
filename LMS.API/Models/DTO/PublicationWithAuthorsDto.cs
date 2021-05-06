using System;
using System.Collections.Generic;
using LMS.API.Models.Entities;

namespace LMS.API.Models.DTO
{
    public class PublicationWithAuthorsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublicationDate { get; set; }
        
        public DifficultyLevel Level { get; set; }
        
        public string TypeName { get; set; }
        public int TypeId { get; set; }
        
        public string SubjectName { get; set; }
        public int SubjectId { get; set; }
        
        public ICollection<AuthorDto> Authors { get; set; }
    }
}
