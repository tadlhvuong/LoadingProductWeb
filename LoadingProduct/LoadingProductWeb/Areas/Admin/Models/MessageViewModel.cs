using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TCVWeb.Areas.Admin.Models
{
    public class ModalFormResult
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class FileUploadResult
    {
        public string[] initialPreview { get; set; }
        public object[] initialPreviewConfig { get; set; }
    }

    public class SelectItemModel
    {
        public int id { get; set; }
        public string text { get; set; }
        public bool selected { get; set; }
    }

    public class SelectExportModel
    {
        public int id { get; set; }
        public string text { get; set; }
        public int? parentId { get; set; }
        public string parentText { get; set; }
        public bool selected { get; set; }
        public bool selectedParent { get; set; }
    }
}
