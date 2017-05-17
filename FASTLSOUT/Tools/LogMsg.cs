using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EltApplication.Tools
{
    class LogMsg
    {
        //日志路径
        String logPath;
        public LogMsg(String logPath)
        {
            this.logPath = logPath;
        }
        //输出信息
        public void ShowMsg(string str)
        {
            try
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(logPath, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + str.Trim());
                }

            }
            catch (Exception e)
            {
                //ShowMsg("发送数据错误……请检查连接或网络状况！");
                // connectBZ = false;
            }
        }
    }
}
