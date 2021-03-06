﻿using System;
using System.Runtime;
using System.Net.Sockets;

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
                new WebSocket(8080).StartAsync();
            }
            catch (Exception e)
            {
                Logger.Log.Error("MiCore.Bootstrap", e);
            }
        }
    }
}
