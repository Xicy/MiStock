using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MiCore
{
    internal partial class WebSocket
    {
        //TODO:Cancel Token Add

        private const string SourceName = "MiCore.SocketTest";

        public static void Start()
        {
            StartListenerAsync();
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
                Logger.Log.Info(SourceName, "Client has connected");

                using (var networkStream = tcpClient.GetStream())
                {
                    var buffer = new byte[4096];
                    Logger.Log.Info(SourceName, "Reading from client");
                    var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                    var request = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    Logger.Log.Debug(SourceName, "Client wrote \n{0}", request);
                    var serverResponseBytes = Encoding.UTF8.GetBytes("Hello from server");
                    await networkStream.WriteAsync(serverResponseBytes, 0, serverResponseBytes.Length);
                    Logger.Log.Info(SourceName, "Response has been written");
                }

            }

        }
    }
}