using System;
using System.Collections.Generic;

namespace MiCore
{
    public partial class Logger
    {
        public static Logger Log = new Logger(
            new DiskConsole(
                Level.All
#if !DEBUG
                    ^ Client.Logger.Level.Debug
#endif
            )
        );

        private readonly IList<IDiskServices> _disks;
        public Logger(params IDiskServices[] disks)
        {
            _disks = new List<IDiskServices>();
            AddDiskService(disks);
        }

        public void AddDiskService(params  IDiskServices[] disks)
        {
            foreach (var disk in disks)
            {
                _disks.Add(disk);
            }   
        }

        private void Write(Level level, string source, object data)
        {
            foreach (var disk in _disks)
                disk.Write(level, source, data);
        }

        public void Debug(string source, object data)
        {
            Write(Level.Debug, source, data);
        }
        public void Debug(string source, string format, params object[] data)
        {
            Write(Level.Debug, source, String.Format(format, data));
        }
        public void Debug(string source, IFormatProvider provider, string format, params object[] data)
        {
            Write(Level.Debug, source, String.Format(provider, format, data));
        }

        public void Info(string source, object data)
        {
            Write(Level.Info, source, data);
        }
        public void Info(string source, string format, params object[] data)
        {
            Write(Level.Info, source, String.Format(format, data));
        }
        public void Info(string source, IFormatProvider provider, string format, params object[] data)
        {
            Write(Level.Info, source, String.Format(provider, format, data));
        }

        public void Warn(string source, object data)
        {
            Write(Level.Warn, source, data);
        }
        public void Warn(string source, string format, params object[] data)
        {
            Write(Level.Warn, source, String.Format(format, data));
        }
        public void Warn(string source, IFormatProvider provider, string format, params object[] data)
        {
            Write(Level.Warn, source, String.Format(provider, format, data));
        }

        public void Error(string source, object data)
        {
            Write(Level.Error, source, data);
        }
        public void Error(string source, string format, params object[] data)
        {
            Write(Level.Error, source, String.Format(format, data));
        }
        public void Error(string source, IFormatProvider provider, string format, params object[] data)
        {
            Write(Level.Error, source, String.Format(provider, format, data));
        }
    }
}