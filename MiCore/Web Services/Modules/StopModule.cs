using System;
using System.Text.RegularExpressions;

namespace MiCore
{
    internal partial class WebSocket
    {
        public class StopModule : IWebModule
        {
            public string RegexPath => @"^\/stop$";
            public Response Execute(WebSocket socket, Request request)
            {
                socket.Stop();
                return new Response(404);
            }
        }
    }
}