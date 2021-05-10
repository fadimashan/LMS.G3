using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.API.Models.Entities;

namespace LMS.API.DtoModels
{
    public class PublicationDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublicationDate { get; set; }

        public DifficultyLevel Level { get; set; }

        public int TypeId { get; set; }
        public PublicationType Type { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}
