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
            private readonly Dictionary<string, string> _kvData;

            public string Method => _kvData["method"];
            public string Path => _kvData["path"].Substring(0, _kvData["path"].Length - 9);
            public string Content => _kvData.ContainsKey("content-length") ? _data.Substring(_data.Length - int.Parse(_kvData["content-length"]), int.Parse(_kvData["content-length"])) : "";

            public Request(string data)
            {
                _kvData = new Dictionary<string, string>();
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
                            _kvData.Add("method", match.Groups["Key"].Value.ToLowerInvariant());
                            _kvData.Add("path", match.Groups["Value"].Value);
                            break;
                        default: _kvData.Add(match.Groups["Key"].Value.ToLowerInvariant(), match.Groups["Value"].Value); break;
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