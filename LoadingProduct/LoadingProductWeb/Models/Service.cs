using System;
namespace LoadingProductWeb.Models
{
    public class Service
    {
        public int id;
        public string title;
        public string preview;
        public string content;
        public string image; 

        public Service(int id, string title, string preview, string content, string image)
        {
            this.id = id;
            this.title = title;
            this.preview = preview;
            this.content = content;
            this.image = image;
        }
    }
}
