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
                //var response = new Response(404);
                if (File.Exists(@"D:\Pictures\Screenshots\" + Match.Groups[1] + "." + Match.Groups[2]))
                {
                    //response = new Response(202, GetFile(), "." + Match.Groups[2]);
                    return new Response(File.OpenRead(@"D:\Pictures\Screenshots\" + Match.Groups[1] + "." + Match.Groups[2]));
                }
                //return response;
                return null;
            }

            private IEnumerable<byte> GetFile()
            {
                var file = new BufferedStream(File.OpenRead(@"D:\Pictures\Screenshots\" + Match.Groups[1] + "." + Match.Groups[2]), BufferSize);
                var Enum = Enumerable.Empty<byte>();
                var buffer = new byte[1024];//1KB
                /*for (var i = 0; i < file.Length; i += 1024)
                {
                    var read = Math.Min(1024, (int) file.Length - i);

                    file.Read(buffer, i, read);

                    Enum = Enum.Concat(buffer.Take(read));
                }
                for (; file.Position < file.Length;)
                {
                    var readed = file.Read(buffer, 0, 1024);
                    Enum = Enum.Concat(buffer.Take(readed));
                }
                */
                byte[] bytes = new byte[BufferSize];
                int numBytesToRead = (int)file.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    // Read may return anything from 0 to numBytesToRead.
                    file.Seek(numBytesRead, 0);
                    int n = file.Read(bytes, 0, BufferSize);

                    // Break when the end of the file is reached.
                    if (n == 0)
                        break;

                    Enum = Enum.Concat(bytes.Take(n));
                    numBytesRead += n;
                    numBytesToRead -= n;
                }

                return Enum;
            }
        }
    }
}