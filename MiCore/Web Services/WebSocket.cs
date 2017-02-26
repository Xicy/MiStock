using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiCore
{
    internal partial class WebSocket
    {
        private const string SourceName = "MiCore.WebSocket";
        private const int BufferSize = 1024 * 256; //8 KByte
        private const long MaxFileSize = 1024 * 1024;//1 Mbyte

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
        }

        private async Task AcceptAsync()
        {
            while (_working)
            {
                var taskClient = await _tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
                var clientAddress = (IPEndPoint)taskClient.Client.RemoteEndPoint;
                Task.Factory.StartNew(() => Listen(taskClient, clientAddress));
            }
            _tcpListener.Stop();
        }

        private void Listen(TcpClient tcpClient, IPEndPoint clientAddress)
        {
            Logger.Log.Info(SourceName, "Client({0}) has connected", clientAddress.Address);
            using (var bufferedStream = new BufferedStream(tcpClient.GetStream(), BufferSize))
            using (var buffer = new MemoryStream())
            {
                var bufferTmp = new byte[BufferSize];
                Logger.Log.Info(SourceName, "Reading from client({0})", clientAddress.Address);

                try
                {
                    if (tcpClient.Available == 0)
                    {
                        bufferedStream.Flush();
                        Logger.Log.Warn(SourceName, "Network data is not available");
                        return;
                    }

                    while (tcpClient.Available != 0)
                    {
                        var contentSize = bufferedStream.Read(bufferTmp, 0, BufferSize);
                        buffer.Write(bufferTmp, 0, contentSize);
                        if (buffer.Length > MaxFileSize)
                        {
                            //TODO: Max File Size Error Page Response
                            Logger.Log.Warn(SourceName, "Client({0}) send bigger than {1}KB data", clientAddress.Address,
                                MaxFileSize);
                            return;
                        }
                    }

                    var request = new Request(buffer);
                    var response = GetModule(request.Path).Execute(this, request);
                    if (!request.Cookies.Contains("Session"))
                        response.AddCookie(new CookieContainer("Session", Guid.NewGuid().ToString()));

                    Logger.Log.Debug(SourceName, "Client({0}) wrote \n{1}", clientAddress.Address, request);
                    response.SendResponseData(bufferedStream);
                }
                catch (Exception e)
                {
                    var socketException = e.InnerException as SocketException;
                    if (socketException != null)
                    {
                        if (socketException.SocketErrorCode == SocketError.ConnectionReset ||
                            socketException.SocketErrorCode == SocketError.ConnectionAborted ||
                            socketException.SocketErrorCode == SocketError.ConnectionRefused)
                        {
                            Logger.Log.Warn(SourceName, "Connection closed from Client({0}) \n{1}",
                                clientAddress.Address, null);
                            return;
                        }
                    }

                    Logger.Log.Error(SourceName, "Client({0}) \n{1}", clientAddress.Address, e);
                }
            }
        }
    }
}