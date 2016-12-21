using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MiCore
{
    internal partial class WebServer
    {
        //TODO:CancellationToken
        private const string SourceName = "MiCore.WebServer";

        private static IList<IWebModule> Modules;



        private static void LoadModules()
        {
            Modules = new List<IWebModule>();
            //TODO:Assembly Resolve add IWebModuls
        }


        public static void Start()
        {
            LoadModules();
            StartListenerAsync();
        }

        private static async void StartListenerAsync()
        {
            var tcpListener = new TcpListener(IPAddress.Any, 0);
            tcpListener.Start();
            Logger.Log.Info(SourceName, $"Started on {((IPEndPoint)tcpListener.LocalEndpoint).Address}:{((IPEndPoint)tcpListener.LocalEndpoint).Port}");
            while (true)
            {
                var tcpClient = await tcpListener.AcceptTcpClientAsync();
                var ip = ((IPEndPoint)tcpClient.Client.LocalEndPoint).Address;
                Logger.Log.Info(SourceName, "Client has connected");

                try
                {
                    //TODO:Wait 
                    using (var networkStream = tcpClient.GetStream())
                    {
                        var buffer = new byte[4096];
                        Logger.Log.Info(SourceName, "Reading from client({0})", ip);
                        var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                        var request = Encoding.UTF8.GetString(buffer, 0, byteCount);
                        Logger.Log.Debug(SourceName, "Client({0}) wrote \n{1}", ip, request);
                        
                        var serverResponseBytes = Encoding.UTF8.GetBytes("Hello from server");

                        await networkStream.WriteAsync(serverResponseBytes, 0, serverResponseBytes.Length);
                        Logger.Log.Info(SourceName, "Response has been written");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log.Error(SourceName, ex);
                }

            }
        }
    }
}