using System;
using System.Threading.Tasks;

namespace MiCore
{
    public partial class Logger
    {
        public class DiskConsole : IDiskServices
        {
            private static int MaxCharLenght = 500;
            private readonly object _lock = new object();

            private readonly Level _visibiltLevel;
            public DiskConsole(Level visibiltLevel = Level.All)
            {
                _visibiltLevel = visibiltLevel;
            }

            public void Write(Level level, string source, object data)
            {
                string write;

                if (data is string)
                {
                    write = data.ToString();
                }
                else if (data is Exception)
                {
                    write = string.Format("{0}\r\n{1}", ((Exception)data).Message, ((Exception)data).StackTrace);
                }
                else throw new ArgumentOutOfRangeException(nameof(data), data, "data type do not support");

                if ((_visibiltLevel & level) == 0) return;
                lock (_lock)
                {
                    var tempColor = Console.ForegroundColor;
                    switch (level)
                    {
                        case Logger.Level.Debug: Console.ForegroundColor = ConsoleColor.Green; break;
                        case Logger.Level.Info: Console.ForegroundColor = ConsoleColor.Cyan; break;
                        case Logger.Level.Warn: Console.ForegroundColor = ConsoleColor.Yellow; break;
                        case Logger.Level.Error: Console.ForegroundColor = ConsoleColor.Red; break;
                    }
                    Console.Write("[{0:HH:mm:ss.fff},{1:000},{2,-5},{3}] {4}{5}".Replace(' ', '\t'), DateTime.Now, Task.CurrentId??0, level, source, write.Substring(0, Math.Min(MaxCharLenght, write.Length)), Environment.NewLine);
                    Console.ForegroundColor = tempColor;
                }
            }
        }
    }
}