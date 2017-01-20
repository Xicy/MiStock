using System.Text.RegularExpressions;

namespace MiCore
{
    internal partial class WebSocket
    {
        public interface IWebModule
        {
            string RegexPath { get; }
            Match Match { set; get; }
            Response Execute(WebSocket socket, Request request);
        }
    }
}