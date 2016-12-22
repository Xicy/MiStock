namespace MiCore
{
    internal partial class WebSocket
    {
        public class IndexModule : IWebModule
        {
            public string RegexPath => @"^(\/|\/index)$";
            public Response Execute(Request request)
            {
                return new Response();
            }
        }
    }
}