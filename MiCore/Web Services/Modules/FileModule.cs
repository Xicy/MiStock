using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MiCore
{
    internal partial class WebSocket
    {
        public class FleModule : IWebModule
        {
            public string RegexPath => @"^\/?(.*?)\.([P|p][N|n][G|g]|[M|m][P|p][4])$";
            public Match Match { set; get; }
            public Response Execute(WebSocket socket, Request request)
            {
                var response = new Response();
                if (File.Exists(@"D:\Pictures\Screenshots\" + Match.Groups[1] + "." + Match.Groups[2]))
                {
                    response.SetStatusCode(200).SetContent(File.OpenRead(@"D:\Pictures\Screenshots\" + Match.Groups[1] + "." + Match.Groups[2]));
                }
                return response;
            }
        }
    }
}