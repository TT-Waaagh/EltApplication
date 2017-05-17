using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using EltApplication.Tools;

namespace EltApplication
{
    public partial class ServiceSIEMEN : ServiceBase
    {
        LogMsg lm = new LogMsg("D:\\EltApplication\\SIEMENSLog.txt");
        dbOperation dbOperationERP = new dbOperation("ERPDBconfig");
        dbOperation dbOperationLocalHost = new dbOperation("DBconfig");

        public ServiceSIEMEN()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            lm.ShowMsg("OnStart=====");
        }

        protected override void OnStop()
        {
            lm.ShowMsg("OnStop====="); 
        }

        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //调用 线程1  获取SILOBOUT中的值，插入中间数据库。
            //Task task1 = Task.Factory.StartNew(() => getErpDatabase());
        }
    }
}
