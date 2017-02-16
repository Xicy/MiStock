using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MiCore
{
    internal partial class WebSocket
    {
        public class Request : IDisposable
        {
            private string Header;
            public readonly Dictionary<string, string> Data;
            public string Method => Data["method"];
            public string Path => Data["path"];
            public IEnumerable<byte> Content;
            public CookieCollection Cookies => Data.ContainsKey("cookie") ? CookieCollection.ToCookieCollection(Data["cookie"]) : null;

            public override string ToString()
            {
                return Header;
            }

            public Request(ref IEnumerable<byte> data)
            {
                var refData = data.ToArray();
                var strData = Encoding.UTF8.GetString(refData);

                Data = new Dictionary<string, string>();

                var regex = new Regex(@"^(?<Key>.*?):?\s(?<Value>.*?)\r?$", RegexOptions.Multiline);
                var regexLen = new Regex(@"^[C|c]ontent-[L|l]ength:\s*?(.*?)$", RegexOptions.Multiline).Match(strData).Groups[1];

                var contentLen = int.Parse(regexLen.Success ? regexLen.Value : "0");

                var headerLen = refData.Length - contentLen;
                Content = refData.Skip(headerLen).Take(contentLen);
                Header = Encoding.UTF8.GetString(refData.Take(headerLen).ToArray());
                var matches = regex.Matches(Header);

                foreach (Match match in matches)
                {
                    switch (match.Groups["Key"].Value.ToLowerInvariant())
                    {
                        case "options":
                        case "get":
                        case "head":
                        case "post":
                        case "put":
                        case "delete":
                            Data.Add("method", match.Groups["Key"].Value.ToLowerInvariant());
                            Data.Add("path", match.Groups["Value"].Value.Substring(0, match.Groups["Value"].Value.Length - 9));
                            break;
                        default: Data.Add(match.Groups["Key"].Value.ToLowerInvariant(), match.Groups["Value"].Value); break;
                    }
                }

            }

            #region Disposing

            protected virtual void Dispose(bool disposing)
            {
                Data.Clear();
                Header = null;
                Content = null;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            
            ~Request()
            {
                Dispose(false);
            }
            #endregion
        }
    }
}