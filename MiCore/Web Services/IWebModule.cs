namespace MiCore
{
    internal partial class WebSocket
    {
        public interface IWebModule
        {
            string RegexPath { get; }
            Response Execute(Request request);
        }
    }
}