using System;
using System.Text;

namespace MiCore
{
    internal partial class WebSocket
    {
        public class Response
        {
            //TODO:Response
            public byte[] ResponseData => Encoding.UTF8.GetBytes(_return);

            private static string _returnBin =
                "<!DOCTYPE html>\r\n<html>\r\n<body>\r\n\r\n<h1>My First Heading</h1>\r\n<p>My first paragraph.</p>\r\n\r\n</body>\r\n</html>";
            private string _return = $"HTTP/1.1 200 OK\r\nDate: {DateTime.Now:r}\r\nServer: {typeof(Bootstrap).Namespace}\r\nLast-Modified: {DateTime.Now:r}\r\nContent-Length: {_returnBin.Length}\r\nContent-Type: text/html\r\nConnection: Closed\r\n\r\n{_returnBin}";
        }
    }
}