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
        //TODO:Cancel Token Add

        private const string SourceName = "MiCore.SocketTest";
        private static IEnumerable<IWebModule> _modules;
        private const int ByteCount = 1024 * 64; //8 Byte
        private static TcpListener _tcpListener;

        public static async void Start()
        {
            try
            {
                _modules = LoadModules();

                _tcpListener = new TcpListener(IPAddress.Any, 8080);
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


        private static IEnumerable<IWebModule> LoadModules()
        {
            var ass = typeof(WebSocket).GetTypeInfo().Assembly;
            foreach (var exportedType in ass.DefinedTypes)
            {
                if (!exportedType.IsInterface && exportedType.ImplementedInterfaces.Any(type => type == typeof(IWebModule)) && exportedType.AsType() != typeof(NotFoundModule))
                {
                    yield return Activator.CreateInstance(exportedType.AsType()) as IWebModule;
                }
            }
        }

        private static IWebModule GetModule(string regex)
        {
            foreach (var module in _modules)
            {
                if (new Regex(module.RegexPath).Match(regex).Success)
                {
                    return module;
                }
            }
            return new NotFoundModule();
        }

        private static async Task ListenAsync()
        {
            try
            {
                while (true)
                {
                    var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                    var clientAddress = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
                    Logger.Log.Info(SourceName, "Client({0}) has connected", clientAddress.Address);

                    using (var networkStream = tcpClient.GetStream())
                    {
                        var buffer = new byte[ByteCount];
                        Logger.Log.Info(SourceName, "Reading from client({0})", clientAddress.Address);
                        var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);

                        var request = new Request(Encoding.UTF8.GetString(buffer, 0, byteCount));
                        Logger.Log.Debug(SourceName, "Client({0}) wrote \n{1}", clientAddress.Address, request);

                        var module = GetModule(request.Path);
                        var serverResponseBytes = module.Execute(request).ResponseData();

                        await networkStream.WriteAsync(serverResponseBytes, 0, serverResponseBytes.Length);
                        Logger.Log.Info(SourceName, "Response has been written");
                    }

                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(SourceName, e);
                await ListenAsync();
            }
        }
    }
}