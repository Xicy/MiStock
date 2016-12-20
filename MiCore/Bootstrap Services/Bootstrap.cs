
namespace MiCore
{
    public class Bootstrap
    {
        public Bootstrap()
        {
            new SocketTest();
            /* Log system
             *  -Develop
             *  
             * Login Server
             *  -Check License with global serial
             *  -Check Update 
             *  -Listen Server for request
             *  
             * Initalize Database
             *  -Password cpu serial
             *  -Check Database file if not exist create with default values
             *  -Load All Modules
             *  
             * Initalize Modules for Api
             *  -Reach all module other module
             *  -
             * 
             * Create WebServer
             *  -Check files from database if not exist request from Server
             *  -Listen for request 
             */
        }
    }
}
