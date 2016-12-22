using System.Text;

namespace MiCore
{
    internal partial class WebSocket
    {
        public class Response
        {
            public byte[] ResponseData => Encoding.UTF8.GetBytes("Hello from server");
            protected internal Request RequestData;
        }
    }
}