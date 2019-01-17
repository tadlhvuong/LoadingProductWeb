using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCVWeb.Areas.Admin.Models
{
    public class FileUploadModel
    {
        public int? Id { get; set; }

        public string FileName { get; set; }

        public string FileData { get; set; }
    }
}
