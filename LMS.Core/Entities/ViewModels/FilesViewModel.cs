using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Entities.ViewModels
{
    public class FileDetails
    {
        public string Name { get; set; }

        public DateTime UploadTime { get; set; }

        public string Path { get; set; }

        public string UserName { get; set; }

        public int? CourseId { get; set; }

        public int? ModuleId { get; set; }

        public int? ActivityId { get; set; }

        public string UserId { get; set; }
    }

    public class FilesViewModel
    {
        public List<FileDetails> Files { get; set; }
            = new List<FileDetails>();
    }
}
