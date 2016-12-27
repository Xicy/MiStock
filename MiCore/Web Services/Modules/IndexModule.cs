using System.IO;
using System.Text;

namespace MiCore
{
    internal partial class WebSocket
    {
        public class IndexModule : IWebModule
        {
            public string RegexPath => @"^(\/|\/index)$";
            public Response Execute(Request request)
            {
                return new Response(File.OpenRead(@"D:\Downloads\a.png"));
                //return new Response(Encoding.UTF8.GetBytes("<!DOCTYPE html>\r\n<html>\r\n<body>\r\n\r\n<h1>My First Heading</h1>\r\n<p>My first paragraph.</p>\r\n\r\n</body>\r\n</html>"), ".html");
            }
        }
    }
}