using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using EltApplication.Model;

namespace EltApplication.Tools
{
    class ClearTableOperation
    {
        LogMsg lm = new LogMsg("D:\\EltApplication\\ClearTable.txt");
        String configPath;
        private ClearTableModel clearTable;
        private void setClearTable(ClearTableModel clearTable)
        {
            this.clearTable = clearTable;
        }
        public ClearTableModel getClearTable()
        {
            return this.clearTable;
        }

        public ClearTableOperation(string xmlName)
        {
            //
            XmlDocument doc = new XmlDocument();
            configPath = Application.StartupPath + "\\" + xmlName + ".xml";
            doc.Load(configPath);
            // 得到根节点bookstore
            XmlNode xn = doc.SelectSingleNode("Server");

            // 得到根节点的所有子节点
            XmlNodeList xnls = xn.ChildNodes;
            ClearTableModel ctm = new ClearTableModel();
            foreach (XmlNode xn1 in xnls)
            {
                String strName = xn1.Name;
                String strValue = xn1.InnerText;
                setModel(strName, strValue, ctm);
            }
            setClearTable(ctm);
        }
        private void setModel(String strName, String strValue, ClearTableModel ctm)
        {
            switch (strName)
            {
                case "FASTLSIN":
                    ctm.Fastlsinday = strValue;
                    break;
                case "FASTLSOUT":
                    ctm.Fastlsoutday = strValue;
                    break;
                case "SILOBIN":
                    ctm.Silobinday = strValue;
                    break;
                case "SILOBOUT":
                    ctm.Siloboutday = strValue;
                    break;
                case "SILOMIN":
                    ctm.Silominday = strValue;
                    break;
                case "SILOMOUT":
                    ctm.Silomoutday = strValue;
                    break;
            }
        }
        /*
         * 清理strDays之前的数据，strDays 是保留数据的时间 
         * 例：strDays=1 
         * 是指清理1天前的数据 
         */
        public void ClearSiloBOutTable(String strName, String strDays, dbOperation dbOperationLocalHost)
        {
            String mDays = strDays;
            bool bDays = false;
            Double iDays = 0;
            try
            {
                iDays = Double.Parse(mDays.Trim());
                bDays = true;
            }
            catch (Exception ex)
            {
                lm.ShowMsg("请检查" + configPath + " 配置文件下 " + strName +
                    "节点的保留数据天数是否正确! ");
            }
            if (bDays)
            {
                DateTime dtClearDate = DateTime.Now.AddDays(0 - iDays);
                try
                {
                    JsonOperation joDom = new JsonOperation();

                    bool b = dbOperationLocalHost.DB_Delete("delete from P_SILO_B_OUT where date_in<to_date('" + dtClearDate + "','YYYY-MM-DD HH24:MI:SS')");
                    if (b)
                    {
                        lm.ShowMsg("清除 P_SILO_B_OUT 表成功.");
                    }
                    else {
                        lm.ShowMsg("清除 P_SILO_B_OUT 表成功.");
                    }
                    
                }
                catch(Exception ex)
                {
                    lm.ShowMsg("清除 P_SILO_B_OUT 表-异常！错误提示："+ex.ToString());
                }
            }
        }
    }

}
