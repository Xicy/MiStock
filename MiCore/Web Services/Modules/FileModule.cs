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
                var response = new Response(404);
                if (File.Exists(@"D:\Pictures\Screenshots\" + Match.Groups[1] + "." + Match.Groups[2]))
                {
                    response = new Response(202, GetFile(), "." + Match.Groups[2]);
                }
                return response;
            }

            private IEnumerable<byte> GetFile()
            {
                var file = File.OpenRead(@"D:\Pictures\Screenshots\" + Match.Groups[1] + "." + Match.Groups[2]);
                var Enum = Enumerable.Empty<byte>();
                var buffer = new byte[1024];//1KB
                for (var i = 0; i < file.Length; i += 1024)
                {
                    var read = Math.Min(1024, (int) file.Length - i);

                    file.Read(buffer, i, read);

                    Enum = Enum.Concat(buffer.Take(read));
                }
                return Enum;
            }
        }
    }
}