using MiCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    public class Program
    {
        //TODO:System.Net.Socket 4.1.0.0 Copy Location 
        //TODO:Best Buffer 64*1024
        public static void Main(string[] args)
        {
            Logger.Log.AddDiskService(new Logger.DiskFile($@"Log.{DateTime.Now:yy.MM.dd}.txt"));
            Logger.Log.Info("Startup", "\n __  __  ____   ___  _____  ____  ____ \r\n(  \\/  )(_  _) / __)(  _  )(  _ \\( ___)\r\n )    (  _)(_ ( (__  )(_)(  )   / )__) \r\n(_/\\/\\_)(____) \\___)(_____)(_)\\_)(____)\n");
            Bootstrap.Start();

            Console.ReadLine();
        }
    }
}
