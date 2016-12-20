using MiCore;
using System;

namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Logger.Log.AddDiskService(new Logger.DiskFile($@"Log.{DateTime.Now:yy.MM.dd}.txt"));

            Logger.Log.Debug("Program.Main", "Test");
            Logger.Log.Info("Program.Main", "Test");
            Logger.Log.Error("Program.Main", "Test");
            Logger.Log.Warn("Program.Main", "Test");

            Bootstrap.Start();

            Console.ReadLine();
        }
    }
}
