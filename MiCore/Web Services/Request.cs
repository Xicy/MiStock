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
        public class Request : IDisposable
        {
            public readonly Dictionary<string, string> Data;
            public string Method => Data["method"];
            public string Path => Data["path"];
            public byte[] Content;
            public CookieCollection Cookies => Data.ContainsKey("cookie") ? CookieCollection.ToCookieCollection(Data["cookie"]) : new CookieCollection();

            public Request(Stream data)
            {
                data.Seek(0, 0);
                using (var streamReader = new StreamReader(data, Encoding.UTF8))
                {
                    string strData = streamReader.ReadToEnd();

                    Data = new Dictionary<string, string>();

                    var regexLen = new Regex(@"^[C|c]ontent-[L|l]ength:\s*?(.*?)$", RegexOptions.Multiline).Match(strData).Groups[1];

                    var contentLen = int.Parse(regexLen.Success ? regexLen.Value : "0");
                    int headerLen = (int)data.Length - contentLen;


                    if (contentLen > 0)
                    {
                        Content = new byte[contentLen];
                        data.Seek(headerLen, 0);
                        data.Read(Content, 0, contentLen);
                    }

                    data.Seek(0, 0);
                    var stringTmp = streamReader.ReadLine().ToLowerInvariant().Split(' ');
                    Data.Add("method", stringTmp[0]);
                    Data.Add("path", stringTmp[1]);

                    string strTmp;
                    while (!string.IsNullOrEmpty(strTmp = streamReader.ReadLine()))
                    {
                        stringTmp = strTmp.Split(':');
                        Data.Add(stringTmp[0].ToLowerInvariant(), stringTmp[1]);
                    }
                }

            }

            public override string ToString()
            {
                return Data.Aggregate("", (current, header) => current + header.Key + (header.Value != null ? $":{header.Value}" : "") + Environment.NewLine);
            }

            #region Disposing

            protected virtual void Dispose(bool disposing)
            {
                Data.Clear();
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