namespace MiCore
{
    public class Global
    {
        #region Statics
        public static Global Instance = new Global();

        public static Logger Log = new Logger(
            new Logger.DiskConsole(
                Logger.Level.All
#if !DEBUG
                    ^ Client.Logger.Level.Debug
#endif
            ),
            new Logger.DiskDatabase(),
            new Logger.DiskFile()
            );


        //Server
        public static string ServerAdress => "127.0.0.1:444";

        //Database
        public static string DatabaseFileName => "Data.dat";
        public static string DatabaseConnectionString => $"filename={DatabaseFileName};"; //TODO:Add Password
        //public LiteDatabase Database;

        //Licence
        private static string _serial;
        public static string Serial => _serial ?? (_serial = WindowsManagementIdentifier("Win32_Processor", "ProcessorId"));


        private static string WindowsManagementIdentifier(string wmiClass, string wmiProperty)
        {
            //TODO: CrossPlatform Cpu ID
            /*string result = "";
            var moc = new ManagementClass(wmiClass).GetInstances();
            foreach (var o in moc)
            {
                var mo = (ManagementObject)o;
                if (result != "") continue;
                try
                {
                    result = mo[wmiProperty].ToString();
                    break;
                }
                catch
                {
                    // ignored
                }
            }
            return result;*/

            return "";
        }
        #endregion



        public Global()
        {
            //Database = new LiteDatabase(DatabaseConnectionString);
        }
    }
}