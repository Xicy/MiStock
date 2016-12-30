using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiCore
{
    internal partial class WebSocket
    {
        private const string SourceName = "MiCore.WebSocket";
        private const int ByteCount = 1024 * 64; //8 Byte

        #region ModuleLoader
        private static List<IWebModule> _modules;
        private static IEnumerable<IWebModule> Modules
        {
            get
            {
                if (_modules != null) return _modules;
                _modules = new List<IWebModule>();
                var ass = typeof(WebSocket).GetTypeInfo().Assembly;
                foreach (var exportedType in ass.DefinedTypes)
                {
                    if (!exportedType.IsInterface && exportedType.ImplementedInterfaces.Any(type => type == typeof(IWebModule)) && exportedType.AsType() != typeof(NotFoundModule))
                    {
                        _modules.Add(Activator.CreateInstance(exportedType.AsType()) as IWebModule);
                    }
                }
                return _modules;
            }
        }
        private static IWebModule GetModule(string regex)
        {
            foreach (var module in Modules)
            {
                if (new Regex(module.RegexPath).Match(regex).Success)
                {
                    return module;
                }
            }
            return new NotFoundModule();
        }
        #endregion

        private bool _working;
        private TcpListener _tcpListener;
        private readonly IPAddress _ip;
        private readonly int _port;

        public WebSocket(IPEndPoint ipEp) : this(ipEp.Address, ipEp.Port) { }
        public WebSocket(int port) : this(IPAddress.Any, port) { }
        public WebSocket(IPAddress ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        public void Start()
        {
            StartAsync().Wait();
        }
        public async Task StartAsync()
        {
            try
            {
                _working = true;
                _tcpListener = new TcpListener(_ip, _port);
                var serverAddress = (IPEndPoint)_tcpListener.LocalEndpoint;

                _tcpListener.Start();
                Logger.Log.Info(SourceName, $"Started on {serverAddress.Address}:{serverAddress.Port}");

                await ListenAsync();
            }
            catch (Exception e)
            {
                Logger.Log.Error(SourceName, e);
            }
        }

        public void Stop()
        {
            _working = false;
            _tcpListener.Stop();
        }

        private async Task ListenAsync()
        {
            while (_working)
            {
                var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                var clientAddress = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
                Logger.Log.Info(SourceName, "Client({0}) has connected", clientAddress.Address);

                try
                {
                    using (var networkStream = tcpClient.GetStream())
                    {
                        var buffer = new byte[ByteCount];
                        Logger.Log.Info(SourceName, "Reading from client({0})", clientAddress.Address);
                        var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                        
                        var request = new Request(Encoding.UTF8.GetString(buffer, 0, byteCount));
                        Logger.Log.Debug(SourceName, "Client({0}) wrote \n{1}", clientAddress.Address, request);

                        var serverResponseBytes = GetModule(request.Path).Execute(this, request).ResponseData();

                        await networkStream.WriteAsync(serverResponseBytes, 0, serverResponseBytes.Length);
                    }
                }
                catch (Exception e)
                {
                    Logger.Log.Error(SourceName, "Client({0}) \n{1}", clientAddress.Address, e);
                }
            }
        }
    }
}