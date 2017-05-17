using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using EltApplication.Tools;
using System.Threading.Tasks;

namespace EltApplication
{
    public partial class ServiceSILOMIN : ServiceBase
    {
        LogMsg lm = new LogMsg("D:\\EltApplication\\SILOMINLog.txt");
        dbOperation dbOperationERP = new dbOperation("ERPDBconfig");
        dbOperation dbOperationLocalHost = new dbOperation("DBconfig");
        public ServiceSILOMIN()
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
            Task task1 = Task.Factory.StartNew(() => getErpDatabase());
        }
        //获取SILOBOUT中的值，插入中间数据库
        private void getErpDatabase()
        {
            try
            {
                //读取ERP数据库
                DataTable dt = new DataTable();
                String strSelect = "SELECT * from P_SILO_M_IN WHERE STATUS = '0' order by date_in";
                dt = dbOperationERP.DB_Find(strSelect);

                if (dt == null)// null  数据库连接问题！
                {
                    lm.ShowMsg("取数据错误！=============请检查与ERP数据库的连接！");
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
                            bool setTag = setLocalhostDatabase(strData, strStatus, strId);
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
        //写入本地数据库数据
        private bool setLocalhostDatabase(String strData, String strStatus, String strId)
        {
            try
            {
                //将记录写入本地数据库
                bool addFlag = dbOperationLocalHost.Db_AddNew("INSERT INTO P_SILO_M_IN(DATA,DATE_IN,STATUS,ERPID) VALUES('" + strData + "',to_date('" + DateTime.Now.ToString() + "','YYYY-MM-DD HH24:MI:SS'),'0','" + strId + "')");
                if (addFlag)
                {
                    lm.ShowMsg("ERP数据： P_SILO_M_IN 表 ID=" + strId + " 记录写入本地数据库 P_SILO_M_IN 表 成功。");
                    return true;
                }
                else
                {
                    lm.ShowMsg("写入本地数据库失败。");
                    return false;
                }
            }
            catch (Exception ex)
            {
                lm.ShowMsg("写入本地数据库-异常！" + ex.ToString());
                return false;
            }
        }
        //给ERP返回读取数据
        private void updateLocalhostDatabase(String strId, String strDateOut, String strStatus)
        {
            try
            {
                String strUpdataSql = "UPDATE P_SILO_M_IN SET DATE_OUT = to_date('" + strDateOut + "','YYYY-MM-DD HH24:MI:SS'),STATUS='" + strStatus + "' WHERE ID='" + strId + "'";
                bool addFlag = dbOperationERP.DB_Update(strUpdataSql);
                if (addFlag)
                {
                    lm.ShowMsg("更新ERP数据库 P_SILO_M_IN 表 ID= " + strId + " 数据成功。");
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
