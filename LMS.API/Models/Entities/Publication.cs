using System;
using System.Collections.Generic;

namespace LMS.API.Models.Entities
{
    public class Publication
    {
        public int Id { get; set; }
        public string Title  { get; set; }
        public string Description  { get; set; }
        public DateTime PublicationDate { get; set; }
        
        public DifficultyLevel Level { get; set; }
        
        public int TypeId { get; set; }
        public PublicationType Type { get; set; }
        
        public int SubjectId  { get; set; }
        public Subject Subject  { get; set; }
        
        public ICollection<Author> Authors { get; set; }
    }
}
