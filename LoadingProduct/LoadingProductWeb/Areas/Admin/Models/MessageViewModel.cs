using System.ComponentModel.DataAnnotations;

namespace LoadingProductWeb.Admin.Models
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
    
}
