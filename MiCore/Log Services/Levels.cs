using System;

namespace MiCore
{
    public partial class Logger
    {
        [Flags]
        public enum Level : byte
        {
            Off = byte.MinValue,    //0

            Debug = 1 << 0,         //1
            Info = 1 << 1,          //2
            Warn = 1 << 2,          //4
            Error = 1 << 3,         //8

            All = byte.MaxValue,    //255
        }


    }
}