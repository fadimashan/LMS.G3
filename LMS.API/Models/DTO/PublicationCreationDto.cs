using System;
using System.Collections.Generic;
using LMS.API.Models.Entities;

namespace LMS.API.Models.DTO
{
    public class PublicationCreationDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublicationDate { get; set; }
        
        public DifficultyLevel Level { get; set; }
        
        public int? TypeId { get; set; }
        public string TypeName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }

        public ICollection<AuthorCreationDto> Authors = new List<AuthorCreationDto>();
    }
}
