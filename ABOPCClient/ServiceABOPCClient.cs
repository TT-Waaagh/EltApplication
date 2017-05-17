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
using EltApplication;

namespace ABOPCClient
{
    partial class ServiceABOPCClient : ServiceBase
    {
        LogMsg lm = new LogMsg("D:\\EltApplication\\ABOPCClientLog.txt");
        dbOperation dbOperationERP = new dbOperation("ERPDBconfig");
        dbOperation dbOperationLocalHost = new dbOperation("DBconfig");

        public ServiceABOPCClient()
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
            //调用 线程1  获取SILOBOUT中的值，写入PLC数据。
            Task task1 = Task.Factory.StartNew(() => getLohostDatabase());
        }
        //获取SILOBOUT中的值，写入PLC数据
        private void getLohostDatabase()
        {
            try
            {
                //读取中间数据库
                DataTable dt = new DataTable();
                String strSelect = "SELECT * from P_FAST_LS_IN WHERE STATUS = '0' order by date_in";
                dt = dbOperationLocalHost.DB_Find(strSelect);

                if (dt == null)// null  数据库连接问题！
                {
                    lm.ShowMsg("取数据错误！=============请检查与中间数据库的连接！");
                }
                else
                {
                    if (dt.Rows.Count > 0)//有新记录
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            String strId = dt.Rows[i]["ID"].ToString();
                            String strData = dt.Rows[i]["DATA"].ToString();
                            String strStatus = dt.Rows[i]["STATUS"].ToString();
                            bool setTag = setPlcData(strData, strStatus, strId);
                            if (setTag)
                            {
                                updateLocalhostDatabase(strId, DateTime.Now.ToString(), "1");
                            }
                        }
                    }
                }
            }
            catch
            {
                lm.ShowMsg("读取数据库-异常！");
            }
        }
        //写入PLC数据
        private bool setPlcData(String strData, String strStatus, String strId)
        {
            try
            {
                //将记录写入PLC
                bool addFlag = dbOperationLocalHost.Db_AddNew("INSERT INTO P_FAST_LS_IN(DATA,DATE_IN,STATUS,ERPID) VALUES('" + strData + "',to_date('" + DateTime.Now.ToString() + "','YYYY-MM-DD HH24:MI:SS'),'0','" + strId + "')");
                if (addFlag)
                {
                    lm.ShowMsg("中间数据： P_FAST_LS_IN 表 ID=" + strId + " 记录写入 plc 成功。");
                    return true;
                }
                else
                {
                    lm.ShowMsg("写入plc失败。");
                    return false;
                }
            }
            catch (Exception ex)
            {
                lm.ShowMsg("写入plc-异常！" + ex.ToString());
                return false;
            }
        }
        //给中间数据库返回写入plc状态
        private void updateLocalhostDatabase(String strId, String strDateOut, String strStatus)
        {
            try
            {
                String strUpdataSql = "UPDATE P_FAST_LS_IN SET DATE_OUT = to_date('" + strDateOut + "','YYYY-MM-DD HH24:MI:SS'),STATUS='" + strStatus + "' WHERE ID='" + strId + "'";
                bool addFlag = dbOperationLocalHost.DB_Update(strUpdataSql);
                if (addFlag)
                {
                    lm.ShowMsg("更新中间数据库 P_FAST_LS_IN 表 ID= " + strId + " 数据成功。");
                }
                else
                {
                    lm.ShowMsg("更新数据库失败。");
                }
            }
            catch
            {
                lm.ShowMsg("更新数据库-异常！");
            }
        }
    }
}
