using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MiCore
{
    public class SocketTest
    {

        private const string SourceName = "MiCore.SocketTest";

        public SocketTest()
        {
            StartListenerAsync();
        }

        private async void StartListenerAsync()
        {
            await Task.Run(async () =>
            {
                var tcpListener = new TcpListener(IPAddress.Any, 0);
                tcpListener.Start();
                Global.Log.Info(SourceName, $"Started on {((IPEndPoint)tcpListener.LocalEndpoint).Address}:{((IPEndPoint)tcpListener.LocalEndpoint).Port}");
                while (true)
                {
                    var tcpClient = await tcpListener.AcceptTcpClientAsync();
                    Global.Log.Info(SourceName, "Client has connected");
                    var task = StartHandleConnectionAsync(tcpClient);
                    // if already faulted, re-throw any error on the calling context
                    if (task.IsFaulted)
                        task.Wait();
                }
            });
        }

        private async Task StartHandleConnectionAsync(TcpClient tcpClient)
        {
            var connectionTask = HandleConnectionAsync(tcpClient);
            try
            {
                await connectionTask;
            }
            catch (Exception ex)
            {
                Global.Log.Error(SourceName, ex);
            }
        }

        private Task HandleConnectionAsync(TcpClient tcpClient)
        {
            return Task.Run(async () =>
            {
                using (var networkStream = tcpClient.GetStream())
                {
                    var buffer = new byte[4096];
                    Global.Log.Info(SourceName, "Reading from client");
                    var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                    var request = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    Global.Log.Debug(SourceName, "Client wrote \n{0}", request);
                    var serverResponseBytes = Encoding.UTF8.GetBytes("Hello from server");
                    await networkStream.WriteAsync(serverResponseBytes, 0, serverResponseBytes.Length);
                    Global.Log.Info(SourceName, "Response has been written");
                }
            });
        }
    }
}