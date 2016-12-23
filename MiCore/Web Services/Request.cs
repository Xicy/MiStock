using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MiCore
{
    internal partial class WebSocket
    {
        public class Request
        {
            private readonly string _data;

            public readonly Dictionary<string, string> Data;
            public string Method => Data["method"];
            public string Path => Data["path"].Substring(0, Data["path"].Length - 9);
            public string Content => Data.ContainsKey("content-length") ? _data.Substring(_data.Length - int.Parse(Data["content-length"]), int.Parse(Data["content-length"])) : "";

            public Request(string data)
            {
                Data = new Dictionary<string, string>();
                _data = data;
                var regex = new Regex(@"^(?<Key>.*?):?\s(?<Value>.*?)\r?$", RegexOptions.Multiline);
                var matches = regex.Matches(_data);
                foreach (Match match in matches)
                {
                    switch (match.Groups["Key"].Value.ToLowerInvariant())
                    {
                        case "get":
                        case "post":
                        case "put":
                        case "delete":
                            Data.Add("method", match.Groups["Key"].Value.ToLowerInvariant());
                            Data.Add("path", match.Groups["Value"].Value);
                            break;
                        default: Data.Add(match.Groups["Key"].Value.ToLowerInvariant(), match.Groups["Value"].Value); break;
                    }
                }
            }

            public override string ToString()
            {
                return _data;
            }
        }
    }
}