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
    public partial class ServiceFASTLSOUT : ServiceBase
    {
        LogMsg lm = new LogMsg("D:\\EltApplication\\FASTLSOUTLog.txt");
        dbOperation dbOperationERP = new dbOperation("ERPDBconfig");
        dbOperation dbOperationLocalHost = new dbOperation("DBconfig");

        public ServiceFASTLSOUT()
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
            //调用 线程1  获取SILOBOUT中的值，插入ERP数据库。
            Task task1 = Task.Factory.StartNew(() => getLocalhostDatabase());
        }
        //获取SILOBOUT中的值，插入ERP数据库
        private void getLocalhostDatabase()
        {
            try
            {
                //读取本地数据库
                DataTable dt = new DataTable();
                String strSelect = "SELECT * from P_FAST_LS_OUT WHERE STATUS = '0' order by date_in";
                dt = dbOperationLocalHost.DB_Find(strSelect);

                if (dt == null)// null  数据库连接问题！
                {
                    lm.ShowMsg("取数据错误！=============请检查与本地数据库的连接！");
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
                            bool setTag = setERPDatabase(strData, strStatus, strId);
                            if (setTag)
                            {
                                updateERPDatabase(strId, DateTime.Now.ToString(), "1");
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
        //写入ERP数据库数据
        private bool setERPDatabase(String strData, String strStatus, String strId)
        {
            try
            {
                //将记录写入本地数据库
                bool addFlag = dbOperationERP.Db_AddNew("INSERT INTO P_FAST_LS_OUT(DATA,DATE_IN,STATUS) VALUES('" + strData + "',to_date('" + DateTime.Now.ToString() + "','YYYY-MM-DD HH24:MI:SS'),'0')");
                if (addFlag)
                {
                    lm.ShowMsg("本地数据： P_FAST_LS_OUT 表 ID=" + strId + " 记录写入ERP数据库 P_FAST_LS_OUT 表 成功。");
                    return true;
                }
                else
                {
                    lm.ShowMsg("写入ERP数据库失败。");
                    return false;
                }
            }
            catch
            {
                lm.ShowMsg("写入ERP数据库-异常！");
                return false;
            }
        }
        //给本地数据库返回读取数据
        private void updateERPDatabase(String strId, String strDateOut, String strStatus)
        {
            try
            {
                String strUpdataSql = "UPDATE P_FAST_LS_OUT SET DATE_OUT = to_date('" + strDateOut + "','YYYY-MM-DD HH24:MI:SS'),STATUS='" + strStatus + "' WHERE ID='" + strId + "'";
                bool addFlag = dbOperationLocalHost.DB_Update(strUpdataSql);
                if (addFlag)
                {
                    lm.ShowMsg("更新本地数据库 P_FAST_LS_OUT 表 ID= " + strId + " 数据成功。");
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
