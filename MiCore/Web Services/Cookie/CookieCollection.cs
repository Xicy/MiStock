using System;
using System.Collections.Generic;
using System.Linq;

namespace MiCore
{
    internal partial class WebSocket
    {
        public class CookieCollection
        {
            public IEnumerable<CookieContainer> Cookies;

            public string ToResponseData()
            {
                return Cookies.Aggregate("", (current, pair) => current + (pair.ToResponseData() + Environment.NewLine));
            }

        }
    }
}