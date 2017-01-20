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
        private const int ByteCount = 1024 * 8; //8 KByte

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
                _tcpListener.Server.ReceiveBufferSize = ByteCount;
                _tcpListener.Server.SendBufferSize = ByteCount;
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
                        var buffer = Enumerable.Empty<byte>();
                        Logger.Log.Info(SourceName, "Reading from client({0})", clientAddress.Address);

                        if (!networkStream.DataAvailable)
                        {
                            await networkStream.FlushAsync();
                            Logger.Log.Warn(SourceName,"Network data is not available");
                            continue;
                        }

                        while (networkStream.DataAvailable)
                        {
                            var bufferTmp = new byte[ByteCount];
                            var contentSize = await networkStream.ReadAsync(bufferTmp, 0, bufferTmp.Length);
                            buffer = buffer.Concat(bufferTmp.Take(contentSize));
                        }

                        var request = new Request(ref buffer);

                        Logger.Log.Debug(SourceName, "Client({0}) wrote \n{1}", clientAddress.Address, request);

                        var serverResponseBytes = GetModule(request.Path).Execute(this, request).ResponseData();
                        var serverResponseBytesSize = serverResponseBytes.Count();

                        for (var count = serverResponseBytesSize; count > 0; count -= ByteCount)
                        {
                          await networkStream.WriteAsync(serverResponseBytes.Skip(serverResponseBytesSize - count).Take(count).ToArray(), 0, count);
                        }

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