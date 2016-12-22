using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MiCore
{
    internal partial class WebSocket
    {
        //TODO:Cancel Token Add

        private const string SourceName = "MiCore.SocketTest";
        private static IList<IWebModule> _modules;
        private const int ByteCount = 1024 * 64; //8 Byte

        public static void Start()
        {
            try
            {
                _modules = LoadModules().ToList();
                StartListenerAsync();
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

        private static async void StartListenerAsync()
        {
            var tcpListener = new TcpListener(IPAddress.Any, 8080);
            var address = (IPEndPoint)tcpListener.LocalEndpoint;

            tcpListener.Start();

            Logger.Log.Info(SourceName, $"Started on {address.Address}:{address.Port}");
            while (true)
            {
                var tcpClient = await tcpListener.AcceptTcpClientAsync();
                Logger.Log.Info(SourceName, "Client({0}) has connected", address.Address);

                using (var networkStream = tcpClient.GetStream())
                {
                    var buffer = new byte[ByteCount];
                    Logger.Log.Info(SourceName, "Reading from client({0})", address.Address);
                    var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);

                    var request = new Request(Encoding.UTF8.GetString(buffer, 0, byteCount));
                    Logger.Log.Debug(SourceName, "Client({0}) wrote \n{1}", address.Address, request);

                    var module = GetModule(request.Path);
                    var serverResponseBytes = module.Execute(request).ResponseData;

                    await networkStream.WriteAsync(serverResponseBytes, 0, serverResponseBytes.Length);
                    Logger.Log.Info(SourceName, "Response has been written");
                }

            }

        }
    }
}