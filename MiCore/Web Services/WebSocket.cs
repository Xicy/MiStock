using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiCore
{
    internal partial class WebSocket
    {
        //TODO:Cancel Token Add

        private const string SourceName = "MiCore.SocketTest";
        private static IDictionary<string, IWebModule> _modules;
        private const int ByteCount = 1024 * 1024;//1KB

        public static async void Start()
        {
            _modules = await LoadModules();
            StartListenerAsync();
        }

        private static async Task<IDictionary<string, IWebModule>> LoadModules()
        {
            //TODO:Assembly Modules load
            var ret = new Dictionary<string, IWebModule> { { "^[/]$", new TestModule() } };
            return ret;
        }

        private static IWebModule GetModule(string regex)
        {
            foreach (var module in _modules)
            {
                if (new Regex(module.Key).Match(regex).Success)
                {
                    return module.Value;
                }
            }
            return null;
        }

        private static async void StartListenerAsync()
        {
            var tcpListener = new TcpListener(IPAddress.Any, 0);
            var address = ((IPEndPoint)tcpListener.LocalEndpoint);
            tcpListener.Start();

            Logger.Log.Info(SourceName, $"Started on {address.Address}:{((IPEndPoint)tcpListener.LocalEndpoint).Port}");
            while (true)
            {
                var tcpClient = await tcpListener.AcceptTcpClientAsync();
                Logger.Log.Info(SourceName, "Client({0}) has connected", address.Address);

                using (var networkStream = tcpClient.GetStream())
                {
                    var buffer = new byte[ByteCount];
                    Logger.Log.Info(SourceName, "Reading from client({0})", address.Address);
                    var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);

                    var req = new Request(Encoding.UTF8.GetString(buffer, 0, byteCount));
                    Logger.Log.Debug(SourceName, "Client({0}) wrote \n{1}", address.Address, req);

                    var module = GetModule(req.Path ?? "/");
                    var response = module.Get(req);//TODO:Decide Get OR Post

                    var serverResponseBytes = response.ResponseData;

                    await networkStream.WriteAsync(serverResponseBytes, 0, serverResponseBytes.Length);
                    Logger.Log.Info(SourceName, "Response has been written");
                }

            }

        }
    }
}