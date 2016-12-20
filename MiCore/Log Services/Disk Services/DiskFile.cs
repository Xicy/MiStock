using System;
using System.IO;

namespace MiCore
{
    public partial class Logger
    {
        public class DiskFile : IDiskServices
        {
            private readonly object _lock = new object();

            public void Write(Level level, string source, object data)
            {
                string write;
                if (data is string)
                {
                    write = data.ToString();
                }
                else if (data is Exception)
                {
                    write = ((Exception)data).Message;
                }
                else throw new ArgumentOutOfRangeException(nameof(data), data, "data type do not support");

                lock (_lock)
                {
                    File.AppendAllText($@"Log.{DateTime.Now:yy.MM.dd}.txt", string.Format("[{0}] [{1}] [{2}] {3}{4}".Replace(' ', '\t'), DateTime.Now, level, source, write, Environment.NewLine));
                }
            }
        }
    }
}