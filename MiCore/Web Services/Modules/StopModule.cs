using System;
using System.Text.RegularExpressions;

namespace MiCore
{
    internal partial class WebSocket
    {
        public class StopModule : IWebModule
        {
            public string RegexPath => @"^\/stop$";
            public Match Match { set; get; }
            public Response Execute(WebSocket socket, Request request)
            {
                socket.Stop();
                Logger.Log.Info("MiCore.WebSocket.StopModule", "Server Down");
                return null;
                //return new Response(404);
            }
        }
    }
}