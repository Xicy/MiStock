using System.Text.RegularExpressions;

namespace MiCore
{
    internal partial class WebSocket
    {
        public class NotFoundModule : IWebModule
        {
            public string RegexPath => "";
            public Response Execute(WebSocket socket, Request request)
            {
                
                return new Response(404);
            }
        }
    }
}