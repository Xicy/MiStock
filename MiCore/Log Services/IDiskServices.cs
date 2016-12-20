namespace MiCore
{
    public partial class Logger
    {
        public interface IDiskServices
        {
            void Write(Level level, string source, object data);
        }
    }
}