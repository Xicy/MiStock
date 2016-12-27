using System;
using System.IO;

namespace MiCore
{
    public partial class Logger
    {
        public class DiskFile : IDiskServices
        {
            private readonly object _lock = new object();
            private readonly string _filepath;
            public DiskFile(string path)
            {
                _filepath = path;
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
                    write = string.Format("{0}\r\n{1}",((Exception)data).Message,((Exception)data).StackTrace);
                }
                else throw new ArgumentOutOfRangeException(nameof(data), data, "data type do not support");

                lock (_lock)
                {
                    File.AppendAllText(_filepath, string.Format("[{0}] [{1}] [{2}] {3}{4}".Replace(' ', '\t'), DateTime.Now, level, source, write, Environment.NewLine));
                }
            }
        }
    }
}