
namespace LMS.Core.Entities
{
    public class ApplicationUserCourse
    {
        public int CourseId { get; set; }
        public string ApplicationUserId { get; set; }

        public ApplicationUser Student { get; set; }
        public Course Course { get; set; }
    }
}
