namespace MiCore
{
    public partial class Logger
    {
        public class DiskDatabase : IDiskServices
        {
            public void Write(Level level, string source, object data)
            {
                //TODO:Database logging system
                return;
            }
        }
    }
}