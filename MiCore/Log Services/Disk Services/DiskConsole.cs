using System;

namespace MiCore
{
    public partial class Logger
    {
        public class DiskConsole : IDiskServices
        {
            private readonly object _lock = new object();

            private readonly Level _visibiltLevel;
            public DiskConsole(Level visibiltLevel = Level.All)
            {
                _visibiltLevel = visibiltLevel;
            }

            public void ConsoleColorChanger(Level level, string source, string data)
            {
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
                    Console.Write("[{0}] [{1}] [{2}] {3}{4}".Replace(' ', '\t'), DateTime.Now, level, source, data, Environment.NewLine);
                    Console.ForegroundColor = tempColor;
                }
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
                    write = string.Format("{0}\r\n{1}", ((Exception) data).Message, ((Exception) data).StackTrace);
                }
                else throw new ArgumentOutOfRangeException(nameof(data), data, "data type do not support");

                ConsoleColorChanger(level, source, write);
            }
        }
    }
}