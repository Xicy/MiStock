using System.Text.RegularExpressions;

namespace MiCore
{
    internal partial class WebSocket
    {
        public class NotFoundModule : IWebModule
        {
            public string RegexPath => "";
            public Match Match { set; get; }
            public Response Execute(WebSocket socket, Request request)
            {
                //return new Response(404);
                return null;
            }
        }
    }
}