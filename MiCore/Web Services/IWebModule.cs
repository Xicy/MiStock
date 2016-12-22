namespace MiCore
{
    internal partial class WebSocket
    {
        public interface IWebModule
        {
            Response Get(Request request);
            Response Post(Request request);
        }
    }
}