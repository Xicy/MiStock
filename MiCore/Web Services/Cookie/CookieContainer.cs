using System;
using System.IO;

namespace MiCore
{
    internal partial class WebSocket
    {
        public class CookieContainer
        {
            public string Key;
            public string Value;
            public string Path;
            public DateTime Expires;
            public TimeSpan MaxAge;

            public bool Secure;
            public bool HttpOnly;

            public CookieContainer(string key, string value)
            {
                if (key.ToLowerInvariant() == "expires" || key.ToLowerInvariant() == "max-age" || key.ToLowerInvariant() == "path" || key.ToLowerInvariant() == "secure" || key.ToLowerInvariant() == "httponly")
                {
                    throw new ArgumentException("Invalid Key name. Please dont use 'expires', 'max-age', 'path', 'secure', 'httponly'");
                }

                Key = key;
                Value = value;
            }

            public string ToResponseData()
            {
                return $"Set-Cookie:{Key}={Value};{(string.IsNullOrEmpty(Path) ? "" : $" Path={Path};")}{(Expires == DateTime.MinValue ? "" : $" Expires={Expires:r};")}{(MaxAge == TimeSpan.Zero ? "" : $" Max-Age={MaxAge.TotalSeconds};")}{(Secure ? " Secure;" : "")}{(HttpOnly ? " HttpOnly;" : "")}";
            }

        }
    }
}