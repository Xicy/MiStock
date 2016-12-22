namespace MiCore
{
    internal partial class WebSocket
    {
        public class TestModule : IWebModule
        {
            public Response Get(Request request)
            {
                return new Response();
            }

            public Response Post(Request request)
            {
                return new Response();
            }
        }
    }
}