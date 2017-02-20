using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MiCore
{
    public partial class Logger
    {
        public static Logger Log = new Logger(
            new DiskConsole(
                Level.All
#if !DEBUG 
                ^ Logger.Level.Debug
#endif
            ).Write
        );

        public event Action<Level, string, object> Disks;

        public Logger(params Action<Level, string, object>[] disks)
        {
            AddDiskService(disks);
        }

        private void AddDiskService(params Action<Level, string, object>[] disks)
        {
            foreach (var disk in disks)
            {
                Disks += disk;
            }
        }

        private void Write(Level level, [CallerMemberName] string source = "", object data = null)
        {
            Disks(level, source, data);
        }

        public void Debug(string source, object data = null)
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