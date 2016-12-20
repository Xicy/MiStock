using System;

namespace MiCore
{
    public partial class Logger
    {
        private readonly IDiskServices[] _disks;

        public Logger(params IDiskServices[] disks)
        {
            _disks = disks;
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
            Write(Level.Debug, source, string.Format(format, data));
        }
        public void Debug(string source, IFormatProvider provider, string format, params object[] data)
        {
            Write(Level.Debug, source, string.Format(provider, format, data));
        }

        public void Info(string source, object data)
        {
            Write(Level.Info, source, data);
        }
        public void Info(string source, string format, params object[] data)
        {
            Write(Level.Info, source, string.Format(format, data));
        }
        public void Info(string source, IFormatProvider provider, string format, params object[] data)
        {
            Write(Level.Info, source, string.Format(provider, format, data));
        }

        public void Warn(string source, object data)
        {
            Write(Level.Warn, source, data);
        }
        public void Warn(string source, string format, params object[] data)
        {
            Write(Level.Warn, source, string.Format(format, data));
        }
        public void Warn(string source, IFormatProvider provider, string format, params object[] data)
        {
            Write(Level.Warn, source, string.Format(provider, format, data));
        }

        public void Error(string source, object data)
        {
            Write(Level.Error, source, data);
        }
        public void Error(string source, string format, params object[] data)
        {
            Write(Level.Error, source, string.Format(format, data));
        }
        public void Error(string source, IFormatProvider provider, string format, params object[] data)
        {
            Write(Level.Error, source, string.Format(provider, format, data));
        }
    }
}