namespace TCVWeb.Models
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AdditionInfo
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public AdditionInfo(string title, string content)
        {
            this.Title = title;
            this.Content = content;
        }

    }
}
