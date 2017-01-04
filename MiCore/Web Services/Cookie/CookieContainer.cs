using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MiCore
{
    internal partial class WebSocket
    {
        public class CookieContainer
        {
            public KeyValuePair<string, string> Cookies;
            public string Path;
            public DateTime Expires;
            public TimeSpan MaxAge;

            public bool Secure;
            public bool HttpOnly;

            //TODO: expires and max-age tostring()
            public static CookieContainer ToCookie(string cookie)
            {
                var matches = new Regex(@"(?<key>[^\s*]*?)=(?<value>[^;]*)|(?<secure>[S|s]ecure)|(?<httponly>[H|h]ttp[O|o]nly)").Matches(cookie).Cast<Match>();

                var keyvalue = matches.Where(
                    a =>
                        a.Groups["key"].Value.ToLowerInvariant() != "path" &&
                        a.Groups["key"].Value.ToLowerInvariant() != "expires" &&
                        a.Groups["key"].Value.ToLowerInvariant() != "max-age" &&
                        !a.Groups["secure"].Success &&
                        !a.Groups["httponly"].Success
                        ).Select(a => new KeyValuePair<string, string>(a.Groups["key"].Value, a.Groups["value"].Value));

                return new CookieContainer
                {
                    //Cookies = keyvalue,
                    Expires = matches.Count(a => a.Groups["key"].Value.ToLowerInvariant() == "expires") > 0 ? DateTime.Parse(matches.First(a => a.Groups["key"].Value.ToLowerInvariant() == "expires").Groups["value"].Value) : DateTime.MinValue,
                    MaxAge = matches.Count(a => a.Groups["key"].Value.ToLowerInvariant() == "max-age") > 0 ? TimeSpan.FromSeconds(double.Parse(matches.First(a => a.Groups["key"].Value.ToLowerInvariant() == "max-age").Groups["value"].Value)) : TimeSpan.Zero,
                    Path = matches.Count(a => a.Groups["key"].Value.ToLowerInvariant() == "path") > 0 ? matches.First(a => a.Groups["key"].Value.ToLowerInvariant() == "path").Groups["value"].Value : string.Empty,
                    Secure = matches.Any(a => a.Groups["secure"].Success),
                    HttpOnly = matches.Any(a => a.Groups["httponly"].Success)
                };
            }

            public string ToResponseData()
            {
                return $"Set-Cookie:{Cookies.Key}={Cookies.Value};{(string.IsNullOrEmpty(Path) ? "" : $" Path={Path};")}{(Expires == DateTime.MinValue ? "" : $" Expires={Expires:r};")}{(MaxAge == TimeSpan.Zero ? "" : $" Max-Age={MaxAge.TotalSeconds};")}{(Secure ? " Secure;" : "")}{(HttpOnly ? " HttpOnly;" : "")}";
            }

        }
    }
}