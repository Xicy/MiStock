namespace MiCore
{
    internal partial class WebServer
    {
        public interface IWebModule
        {
            Response Get(Request request);
            Response Post(Request request);
        }
    }
}