using System;
using System.Collections.Generic;
using System.Text;

namespace ABOPCClient
{
    class OPCConfig
    {
        #region // 定义全局变量 
        public static string LogPath;
        public static string OPCServerIP;
        public static string OPCServerName;
        #endregion

        public OPCConfig()
        {
            LogPath = System.Configuration.ConfigurationManager.AppSettings["LogPath"];
            OPCServerIP = System.Configuration.ConfigurationManager.AppSettings["OPCServerIP"];
            OPCServerName = System.Configuration.ConfigurationManager.AppSettings["OPCServerName"];
        }
    }
}
