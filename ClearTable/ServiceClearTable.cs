using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using EltApplication.Tools;
using EltApplication.Model;

namespace EltApplication
{
    public partial class ServiceClearTable : ServiceBase
    {
        LogMsg lm = new LogMsg("D:\\EltApplication\\ClearTableLog.txt");
        dbOperation dbOperationERP = new dbOperation("ERPDBconfig");
        dbOperation dbOperationLocalHost = new dbOperation("DBconfig");
        ClearTableOperation cto = new ClearTableOperation("ClearTableconfig");
        public ServiceClearTable()
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
            Task task1 = Task.Factory.StartNew(() => clearFastLsIn(e));
            //调用 线程1  获取SILOBOUT中的值，插入中间数据库。
            Task task2 = Task.Factory.StartNew(() => clearFastLsOut(e));
            //调用 线程1  获取SILOBOUT中的值，插入中间数据库。
            Task task3 = Task.Factory.StartNew(() => clearSILOBIn(e));
            //调用 线程1  获取SILOBOUT中的值，插入中间数据库。
            Task task4 = Task.Factory.StartNew(() => clearSILOBOut(e));
            //调用 线程1  获取SILOBOUT中的值，插入中间数据库。
            Task task5 = Task.Factory.StartNew(() => clearSILOMIn(e));
            //调用 线程1  获取SILOBOUT中的值，插入中间数据库。
            Task task6 = Task.Factory.StartNew(() => clearSILOMOut(e));
        }
        private void clearSILOBIn(System.Timers.ElapsedEventArgs e)
        {
            ClearTableModel ctm = cto.getClearTable();
            // 得到 hour minute second  如果等于某个值就开始执行某个程序。
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;
            int intSecond = e.SignalTime.Second;
            //定制时间； 比如 在10：30 ：00 的时候执行某个函数
            int iHour = 15;
            int iMinute = 52;
            int iSecond = 00;

            // 设置　定制时间开始执行程序
            if (intHour == iHour && intMinute == iMinute && intSecond == iSecond)
            {

                cto.ClearSiloBInTable("SILOBIN", ctm.Silobinday, dbOperationLocalHost);
            }
        }
        private void clearSILOBOut(System.Timers.ElapsedEventArgs e)
        {

            ClearTableModel ctm = cto.getClearTable();
            // 得到 hour minute second  如果等于某个值就开始执行某个程序。
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;
            int intSecond = e.SignalTime.Second;
            //定制时间； 比如 在10：30 ：00 的时候执行某个函数
            int iHour = 15;
            int iMinute = 52;
            int iSecond = 00;

            // 设置　定制时间开始执行程序
            if (intHour == iHour && intMinute == iMinute && intSecond == iSecond)
            {

                cto.ClearSiloBOutTable("SILOBOUT", ctm.Siloboutday, dbOperationLocalHost);
            }
        }
        private void clearSILOMIn(System.Timers.ElapsedEventArgs e)
        {

            ClearTableModel ctm = cto.getClearTable();
            // 得到 hour minute second  如果等于某个值就开始执行某个程序。
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;
            int intSecond = e.SignalTime.Second;
            //定制时间； 比如 在10：30 ：00 的时候执行某个函数
            int iHour = 15;
            int iMinute = 52;
            int iSecond = 00;

            // 设置　定制时间开始执行程序
            if (intHour == iHour && intMinute == iMinute && intSecond == iSecond)
            {

                cto.ClearSiloMINTable("SILOMIN", ctm.Silominday, dbOperationLocalHost);
            }
        }
        private void clearSILOMOut(System.Timers.ElapsedEventArgs e)
        {

            ClearTableModel ctm = cto.getClearTable();
            // 得到 hour minute second  如果等于某个值就开始执行某个程序。
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;
            int intSecond = e.SignalTime.Second;
            //定制时间； 比如 在10：30 ：00 的时候执行某个函数
            int iHour = 15;
            int iMinute = 52;
            int iSecond = 00;

            // 设置　定制时间开始执行程序
            if (intHour == iHour && intMinute == iMinute && intSecond == iSecond)
            {
                cto.ClearSiloMOutTable("SILOMOUT", ctm.Silomoutday, dbOperationLocalHost);
            }
        }
        private void clearFastLsIn(System.Timers.ElapsedEventArgs e)
        {

            ClearTableModel ctm = cto.getClearTable();
            // 得到 hour minute second  如果等于某个值就开始执行某个程序。
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;
            int intSecond = e.SignalTime.Second;
            //定制时间； 比如 在10：30 ：00 的时候执行某个函数
            int iHour = 15;
            int iMinute = 52;
            int iSecond = 00;

            // 设置　定制时间开始执行程序
            if (intHour == iHour && intMinute == iMinute && intSecond == iSecond)
            {

                cto.ClearFastLsInTable("FASTLSIN", ctm.Fastlsinday, dbOperationLocalHost);
            }
        }
        private void clearFastLsOut(System.Timers.ElapsedEventArgs e)
        {

            ClearTableModel ctm = cto.getClearTable();
            // 得到 hour minute second  如果等于某个值就开始执行某个程序。
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;
            int intSecond = e.SignalTime.Second;
            //定制时间； 比如 在10：30 ：00 的时候执行某个函数
            int iHour = 15;
            int iMinute = 52;
            int iSecond = 00;

            // 设置　定制时间开始执行程序
            if (intHour == iHour && intMinute == iMinute && intSecond == iSecond)
            {

                cto.ClearFastLsOutTable("FASTLSOUT", ctm.Fastlsoutday, dbOperationLocalHost);
            }
        }
    }
}
