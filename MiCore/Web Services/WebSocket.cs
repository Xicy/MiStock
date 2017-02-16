using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiCore
{
    internal partial class WebSocket
    {
        private const string SourceName = "MiCore.WebSocket";
        private const int BufferSize = 1024 * 8; //8 KByte

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
                var reg = new Regex(module.RegexPath).Match(regex);
                if (reg.Success)
                {
                    module.Match = reg;
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
                _tcpListener.Server.ReceiveBufferSize = BufferSize;
                _tcpListener.Server.SendBufferSize = BufferSize;
                var serverAddress = (IPEndPoint)_tcpListener.LocalEndpoint;

                _tcpListener.Start();
                Logger.Log.Info(SourceName, $"Started on {serverAddress.Address}:{serverAddress.Port}");

                await AcceptAsync();
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

        private async Task AcceptAsync()
        {
            while (_working)
            {
                var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                var clientAddress = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
                Logger.Log.Info(SourceName, "Client({0}) has connected", clientAddress.Address);
                Task.Factory.StartNew(() => ListenAsync(tcpClient, clientAddress));
            }
        }

        private async Task ListenAsync(TcpClient tcpClient, IPEndPoint clientAddress)
        {
            try
            {
                using (var networkStream = tcpClient.GetStream())
                using (var bufferedStream = new BufferedStream(networkStream, BufferSize))
                {
                    var buffer = Enumerable.Empty<byte>();
                    Logger.Log.Info(SourceName, "Reading from client({0})", clientAddress.Address);

                    if (!networkStream.DataAvailable)
                    {
                        await bufferedStream.FlushAsync();
                        Logger.Log.Warn(SourceName, "Network data is not available");
                        return;
                    }

                    while (networkStream.DataAvailable)
                    {
                        var bufferTmp = new byte[BufferSize];
                        var contentSize = await bufferedStream.ReadAsync(bufferTmp, 0, bufferTmp.Length);
                        buffer = buffer.Concat(bufferTmp.Take(contentSize));
                    }

                    var request = new Request(ref buffer);
                    Logger.Log.Debug(SourceName, "Client({0}) wrote \n{1}", clientAddress.Address, request);
                    GetModule(request.Path).Execute(this, request).SendResponseData(bufferedStream);
                }

            }
            catch (Exception e)
            {
                var socketException = e.InnerException as SocketException;
                if (socketException?.SocketErrorCode == SocketError.ConnectionReset)
                {
                    Logger.Log.Warn(SourceName, "Client({0}) \n{1}", clientAddress.Address, "Connection closed from client");
                    return;
                }
                Logger.Log.Error(SourceName, "Client({0}) \n{1}", clientAddress.Address, e);
            }
        }
    }
}