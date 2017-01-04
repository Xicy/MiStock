
using System;

namespace MiCore
{
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
     *  
     *  Response & Request
     *   -Parser 
     */

    public class Bootstrap
    {
        public static void Start()
        {
            try
            {
                var testCookie = WebSocket.CookieContainer.ToCookie("LSID=DQAAAK…Eaem_vYg; ID=sabri; Path=/accounts; Max-Age=1200; Expires=Thu, 14 Jan 2016 00:23:01 GMT; Secure; HttpOnly; Test=sa; k=1");
                
                var Server1 = new WebSocket(8080);
                var Server2 = new WebSocket(8081);
                Server1.StartAsync();
                Server2.StartAsync();
            }
            catch (Exception e)
            {
                Logger.Log.Error("MiCore.Bootstrap", e);
            }
        }
    }
}
