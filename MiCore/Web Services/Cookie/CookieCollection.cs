using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MiCore
{
    internal partial class WebSocket
    {
        public class CookieCollection
        {
            private IList<CookieContainer> _cookies;
            public IEnumerable<CookieContainer> Cookies => _cookies;

            public CookieCollection()
            {
                _cookies = new List<CookieContainer>();
            }
            public CookieCollection(IList<CookieContainer> cookies)
            {
                _cookies = cookies;
            }

            public bool IsNull() => _cookies.Count <= 0;

            public CookieCollection Add(CookieContainer cookie)
            {
                _cookies.Add(cookie);
                return this;
            }

            public static CookieCollection ToCookieCollection(string cookies)
            {

                var matches = new Regex(@"(?<key>[^\s*]*?)=(?<value>[^;]*)|(?<secure>[S|s]ecure)|(?<httponly>[H|h]ttp[O|o]nly)").Matches(cookies).Cast<Match>();

                var _Expires = matches.Count(a => a.Groups["key"].Value.ToLowerInvariant() == "expires") > 0 ? DateTime.Parse(matches.First(a => a.Groups["key"].Value.ToLowerInvariant() == "expires").Groups["value"].Value) : DateTime.MinValue;
                var _MaxAge = matches.Count(a => a.Groups["key"].Value.ToLowerInvariant() == "max-age") > 0 ? TimeSpan.FromSeconds(double.Parse(matches.First(a => a.Groups["key"].Value.ToLowerInvariant() == "max-age").Groups["value"].Value)) : TimeSpan.Zero;
                var _Path = matches.Count(a => a.Groups["key"].Value.ToLowerInvariant() == "path") > 0 ? matches.First(a => a.Groups["key"].Value.ToLowerInvariant() == "path").Groups["value"].Value : string.Empty;
                var _Secure = matches.Any(a => a.Groups["secure"].Success);
                var _HttpOnly = matches.Any(a => a.Groups["httponly"].Success);

                var keyvalue = matches.Where(
                    a =>
                        a.Groups["key"].Value.ToLowerInvariant() != "path" &&
                        a.Groups["key"].Value.ToLowerInvariant() != "expires" &&
                        a.Groups["key"].Value.ToLowerInvariant() != "max-age" &&
                        !a.Groups["secure"].Success &&
                        !a.Groups["httponly"].Success
                        ).Select(a => new CookieContainer(a.Groups["key"].Value, a.Groups["value"].Value) { Expires = _Expires, HttpOnly = _HttpOnly, MaxAge = _MaxAge, Path = _Path, Secure = _Secure });

                return new CookieCollection(keyvalue.ToList());
            }

            public string ToResponseData()
            {
                return Cookies.Aggregate("", (current, pair) => current + pair.ToResponseData() + Environment.NewLine).TrimEnd('\r', '\n');
            }

        }
    }
}