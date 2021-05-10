using System;

namespace LMS.Core.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime UploadTime { get; set; }

        public string UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public int? CourseId { get; set; }
        public Course Course { get; set; }

        public int? ModuleId { get; set; }
        public Module Module { get; set; }

        public int? ActivityId { get; set; }
        public Activity Activity { get; set; }
    }
}
