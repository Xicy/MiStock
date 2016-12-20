using MiCore;
using System;

namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Global.Log.Debug("Program.Main", "Test");
            Global.Log.Info("Program.Main", "Test");
            Global.Log.Error("Program.Main", "Test");
            Global.Log.Warn("Program.Main", "Test");

            new Bootstrap();

            Console.ReadLine();
        }
    }
}
