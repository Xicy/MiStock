using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MiCore
{
    internal partial class WebSocket
    {
        public class IndexModule : IWebModule
        {
            public string RegexPath => @"^(\/|\/index)$";
            public Match Match { set; get; }
            public Response Execute(WebSocket socket, Request request)
            {
                var response = new Response();
                response.SetStatusCode(200);
                response.SetFileExtention("html");
                response.SetContent(new MemoryStream(Encoding.UTF8.GetBytes("<!DOCTYPE html>\r\n<html>\r\n<body>\r\n\r\n<h1>My First Heading</h1>\r\n<p>My first paragraph.</p>\r\n\r\n</body>\r\n</html>")));
                return response;
            }
        }
    }
}