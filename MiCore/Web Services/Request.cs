namespace MiCore
{
    internal partial class WebSocket
    {
        public class Request
        {
            private string _data;

            public string Path;

            public Request(string data)
            {
                _data = data;
            }

            public override string ToString()
            {
                return _data;
            }
        }
    }
}