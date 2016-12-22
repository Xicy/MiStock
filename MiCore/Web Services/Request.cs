using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MiCore
{
    internal partial class WebSocket
    {
        public class Request
        {
            private string _data;
            private Regex regex;

            private Dictionary<string, string> KVData;

            public string Method => KVData["method"];
            public string Path => KVData["path"].Substring(0, KVData["path"].Length - 9);
            public string PostData => KVData.ContainsKey("content-length") ? _data.Substring(_data.Length - int.Parse(KVData["content-length"]), int.Parse(KVData["content-length"])) : "";

            public Request(string data)
            {
                KVData = new Dictionary<string, string>();
                _data = data;
                regex = new Regex(@"^(?<Key>.*?):?\s(?<Value>.*?)\r?$", RegexOptions.Multiline);
                var matches = regex.Matches(_data);
                foreach (Match match in matches)
                {
                    switch (match.Groups["Key"].Value.ToLowerInvariant())
                    {
                        case "get":
                        case "post":
                        case "put":
                        case "delete":
                            KVData.Add("method", match.Groups["Key"].Value.ToLowerInvariant());
                            KVData.Add("path", match.Groups["Value"].Value);
                            break;
                        default: KVData.Add(match.Groups["Key"].Value.ToLowerInvariant(), match.Groups["Value"].Value); break;
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