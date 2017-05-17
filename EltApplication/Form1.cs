using System;
using System.Data;
using System.Windows.Forms;
using EltApplication.Tools;
using System.Collections.Generic;
using EltApplication.Model;

namespace EltApplication
{
    public partial class Form1 : Form
    {
        LogMsg lm = new LogMsg("D:\\EltApplication\\SILOBINLog.txt");
        dbOperation dbOperationERP = new dbOperation("ERPDBconfig");
        dbOperation dbOperationLocalHost = new dbOperation("DBconfig");
        ClearTableOperation ct = new ClearTableOperation("ClearTableconfig");

        private String strCheckBox1 = "n";
        private String strCheckBox2 = "n";
        private String strCheckBox3 = "n";
        private String strCheckBox4 = "n";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            setERPSILOBMDatabase();
        }
        //写入本地数据库SILOB
        private void setMSILOBDatabase()
        {
            try
            {
                JsonOperation joDom = new JsonOperation();
                DataModel dom1 = new DataModel() { sts = "1" };
                DataModel dom2 = new DataModel() { sts = "0" };
                DataModel dom3 = new DataModel() { sts = "1" };
                DataModel dom4 = new DataModel() { sts = "0" };
                DataSILOB ds = new DataSILOB();
                ds.b01 = dom1;
                ds.b02 = dom2;
                ds.b03 = dom3;
                ds.b04 = dom4;

                String Data = joDom.SILOBToJson(ds);
                String[] ColumnName = new String[3];
                ColumnName[0] = "DATA";
                ColumnName[1] = "DATE_IN";
                ColumnName[2] = "STATUS";

                String[] data1 = new String[3];
                data1[0] = Data;
                data1[1] = DateTime.Now.ToString();
                data1[2] = "1";
                bool addFlag1 = dbOperationLocalHost.Db_AddNew("INSERT INTO P_SILO_B_OUT(DATA,DATE_IN,STATUS) VALUES('" + Data + "',to_date('" + DateTime.Now.ToString() + "','YYYY-MM-DD HH24:MI:SS'),'0')");
                if (addFlag1)
                {
                    lm.ShowMsg("插入数据库成功。");
                }
                else
                {
                    lm.ShowMsg("插入数据库失败。");
                }
            }
            catch
            {
                lm.ShowMsg("插入数据库-异常！");
            }
        }
        //写入远程ERP数据库SILOB
        private void setERPSILOBMDatabase()
        {
            try
            {
                JsonOperation joDom = new JsonOperation();
                DataModel dom1 = new DataModel();
                dom1.sts = strCheckBox1;
                DataModel dom2 = new DataModel();
                dom2.sts = strCheckBox2;
                DataModel dom3 = new DataModel();
                dom3.sts = strCheckBox3;
                DataModel dom4 = new DataModel();
                dom4.sts = strCheckBox4;
                DataSILOB ds = new DataSILOB();
                ds.b01 = dom1;
                ds.b02 = dom2;
                ds.b03 = dom3;
                ds.b04 = dom4;

                String Data = joDom.SILOBToJson(ds);
                String[] ColumnName = new String[3];
                ColumnName[0] = "DATA";
                ColumnName[1] = "DATE_IN";
                ColumnName[2] = "STATUS";

                String[] data1 = new String[3];
                data1[0] = Data;
                data1[1] = DateTime.Now.ToString();
                data1[2] = "1";
                bool addFlag1 = dbOperationERP.Db_AddNew("INSERT INTO P_SILO_B_IN(DATA,DATE_IN,STATUS) VALUES('" + Data + "',to_date('" + DateTime.Now.ToString() + "','YYYY-MM-DD HH24:MI:SS'),'0')");
                if (addFlag1)
                {
                    lm.ShowMsg("插入数据库成功。");
                }
                else
                {
                    lm.ShowMsg("插入数据库失败。");
                }
            }
            catch
            {
                lm.ShowMsg("插入数据库-异常！");
            }
        }
        //写入ERP数据库
        private void getERPDatabase()
        {
            try
            {
                //读取ERP数据库
                DataTable dt = new DataTable();
                String strSelect = "SELECT * from P_SILO_B_IN WHERE STATUS = '0'";
                dt = dbOperationERP.DB_Find(strSelect);
                InOutDataBasicModel model = new InOutDataBasicModel();

                if (dt == null)// null  数据库连接问题！
                {
                    lm.ShowMsg("取数错误！=============请检查本地数据库服务！");
                }
                else
                {
                    if (dt.Rows.Count > 0)//有新记录
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            model.Erpid = dt.Rows[i]["ID"].ToString();
                            model.Data = dt.Rows[i]["DATA"].ToString();
                            DataOperations(model.Data);
                            model.Datein = dt.Rows[i]["DATE_IN"].ToString();
                            model.Dateout = dt.Rows[i]["DATE_OUT"].ToString();
                            model.Status = dt.Rows[i]["STATUS"].ToString();
                            getDatabaseStatus(model.Erpid);
                        }
                    }

                }
            }
            catch
            {
                lm.ShowMsg("读取数据库-异常！");
            }
        }
        //处理数据
        private void DataOperations(String strData)
        {
            //
            JsonOperation jo = new JsonOperation();
            //
            DataSILOB ds = jo.SILOBToModel(strData);
            DataModel dom01 = ds.b01;
            DataModel dom02 = ds.b02;
            DataModel dom03 = ds.b03;
            DataModel dom04 = ds.b04;

            DataModel[] doms = { dom01, dom02, dom03, dom04 };
        }
        //写入成功，改变状态
        private void getDatabaseStatus(String strId)
        {
            try
            {
                String strUpdataSql = "UPDATE P_SILO_B_IN SET DATE_OUT = to_date('" + DateTime.Now.ToString() + "','YYYY-MM-DD HH24:MI:SS'),STATUS='1' WHERE ERPID='" + strId + "'";
                bool addFlag = dbOperationLocalHost.DB_Update(strUpdataSql);
                if (addFlag)
                {
                    lm.ShowMsg("更改数据库成功。");
                }
                else
                {
                    lm.ShowMsg("更改数据库失败。");
                }
            }
            catch
            {
                lm.ShowMsg("读取数据库-异常！");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            setMSILOBDatabase();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataFastLsInModel dfl1 = new DataFastLsInModel();
            dfl1.pn = "a1";
            dfl1.sts = "b1";
            dfl1.tn = "c1";
            DataFastLsInModel dfl2 = new DataFastLsInModel();
            dfl2.pn = "a2";
            dfl2.sts = "b2";
            dfl2.tn = "c2";
            DataFastLsInModel dfl3 = new DataFastLsInModel();
            DataFastLsInModel dfl4 = new DataFastLsInModel();
            DataFastLsInModel dfl5 = new DataFastLsInModel();
            DataFastLsInModel dfl6 = new DataFastLsInModel();
            DataFastLsInModel dfl7 = new DataFastLsInModel();
            DataFastLsInModel dfl8 = new DataFastLsInModel();
            DataFastLsInModel dfl9 = new DataFastLsInModel();
            DataFastLsInModel dfl10 = new DataFastLsInModel();
            DataFastLsInModel dfl11 = new DataFastLsInModel();
            DataFastLsInModel dfl12 = new DataFastLsInModel();
            DataFastLsInModel dfl13 = new DataFastLsInModel();
            DataFastLsInModel dfl14 = new DataFastLsInModel();
            DataFastLsInModel dfl15 = new DataFastLsInModel();
            DataFastLsInModel dfl16 = new DataFastLsInModel();
            DataFastLsInModel dfl17 = new DataFastLsInModel();
            DataFastLsInModel dfl18 = new DataFastLsInModel();
            DataFastLsInModel dfl19 = new DataFastLsInModel();
            DataFastLsInModel dfl20 = new DataFastLsInModel();
            DataFastLsInModel dfl21 = new DataFastLsInModel();
            DataFastLsInModel dfl22 = new DataFastLsInModel();
            DataFastLsInModel dfl23 = new DataFastLsInModel();
            DataFastLsInModel dfl24 = new DataFastLsInModel();
            DataFastLsInModel dfl25 = new DataFastLsInModel();
            DataFastLsInModel dfl26 = new DataFastLsInModel();
            DataFastLsInModel dfl27 = new DataFastLsInModel();
            DataFastLsInModel dfl28 = new DataFastLsInModel();
            DataFastLsInModel dfl29 = new DataFastLsInModel();
            DataFastLsInModel dfl30 = new DataFastLsInModel();
            DataFastLsInModel dfl31 = new DataFastLsInModel();
            DataFastLsInModel dfl32 = new DataFastLsInModel();
            DataFastLsToInModel dflti = new DataFastLsToInModel();
            dflti.k01 = dfl1;
            dflti.k02 = dfl2;
            dflti.k03 = dfl3;
            dflti.k04 = dfl4;
            dflti.k05 = dfl5;
            dflti.k06 = dfl6;
            dflti.k07 = dfl7;
            dflti.k08 = dfl8;
            dflti.k09 = dfl9;
            dflti.k10 = dfl10;
            dflti.k11 = dfl11;
            dflti.k12 = dfl12;
            dflti.k13 = dfl13;
            dflti.k14 = dfl14;
            dflti.k15 = dfl15;
            dflti.k16 = dfl16;
            dflti.k17 = dfl17;
            dflti.k18 = dfl18;
            dflti.k19 = dfl19;
            dflti.k20 = dfl20;
            dflti.k21 = dfl21;
            dflti.k22 = dfl22;
            dflti.k23 = dfl23;
            dflti.k24 = dfl24;
            dflti.k25 = dfl25;
            dflti.k26 = dfl26;
            dflti.k27 = dfl27;
            dflti.k28 = dfl28;
            dflti.k29 = dfl29;
            dflti.k30 = dfl30;
            dflti.k31 = dfl31;
            dflti.k32 = dfl32;

            DataOperation dataOperation = new DataOperation();

            String aaa = dataOperation.FaseLsInToZf(dflti);

            DataFastLsToInModel dfltim = dataOperation.FaseLsInToSz(aaa);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DataOperation dataOperation = new DataOperation();
            DataSILOB ds = new DataSILOB();
            DataModel dm1 = new DataModel();
            dm1.sts = "1";
            ds.b01 = dm1;
            DataModel dm2 = new DataModel();
            dm2.sts = "2";
            ds.b02 = dm2;
            DataModel dm3 = new DataModel();
            dm3.sts = "3";
            ds.b03 = dm3;
            DataModel dm4 = new DataModel();
            dm4.sts = "4";
            ds.b04 = dm4;
            String aaa = dataOperation.SilobInToZf(ds);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            setERPSILOMMDatabase();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            setMSILOMDatabase();
        }
        //写入本地数据库SILOM
        private void setMSILOMDatabase()
        {
            try
            {
                JsonOperation joDom = new JsonOperation();
                DataModel dom1 = new DataModel() { sts = "1" };
                DataModel dom2 = new DataModel() { sts = "0" };
                DataModel dom3 = new DataModel() { sts = "1" };
                DataModel dom4 = new DataModel() { sts = "0" };
                DataModel dom5 = new DataModel() { sts = "1" };
                DataModel dom6 = new DataModel() { sts = "0" };
                DataModel dom7 = new DataModel() { sts = "1" };
                DataModel dom8 = new DataModel() { sts = "0" };
                DataModel dom9 = new DataModel() { sts = "1" };
                DataModel dom10 = new DataModel() { sts = "0" };
                DataModel dom11 = new DataModel() { sts = "1" };
                DataModel dom12 = new DataModel() { sts = "0" };
                DataModel dom13 = new DataModel() { sts = "1" };
                DataModel dom14 = new DataModel() { sts = "0" };
                DataModel dom15 = new DataModel() { sts = "1" };
                DataModel dom16 = new DataModel() { sts = "0" };
                DataModel dom17 = new DataModel() { sts = "1" };
                DataModel dom18 = new DataModel() { sts = "0" };
                DataSILOM ds = new DataSILOM();
                ds.m01 = dom1;
                ds.m02 = dom2;
                ds.m03 = dom3;
                ds.m04 = dom4;
                ds.m05 = dom5;
                ds.m06 = dom6;
                ds.m07 = dom7;
                ds.m08 = dom8;
                ds.m09 = dom9;
                ds.m10 = dom10;
                ds.m11 = dom11;
                ds.m12 = dom12;
                ds.m13 = dom13;
                ds.m14 = dom14;
                ds.m15 = dom15;
                ds.m16 = dom16;
                ds.m17 = dom17;
                ds.m18 = dom18;

                String Data = joDom.SILOBMToJson(ds);

                bool addFlag1 = dbOperationLocalHost.Db_AddNew("INSERT INTO P_SILO_M_OUT(DATA,DATE_IN,STATUS) VALUES('" + Data + "',to_date('" + DateTime.Now.ToString() + "','YYYY-MM-DD HH24:MI:SS'),'0')");
                if (addFlag1)
                {
                    lm.ShowMsg("插入数据库成功。");
                }
                else
                {
                    lm.ShowMsg("插入数据库失败。");
                }
            }
            catch
            {
                lm.ShowMsg("插入数据库-异常！");
            }
        }
        //写入远程ERP数据库SILOm
        private void setERPSILOMMDatabase()
        {
            try
            {
                JsonOperation joDom = new JsonOperation();
                DataModel dom1 = new DataModel() { sts = "1" };
                DataModel dom2 = new DataModel() { sts = "0" };
                DataModel dom3 = new DataModel() { sts = "1" };
                DataModel dom4 = new DataModel() { sts = "0" };
                DataModel dom5 = new DataModel() { sts = "1" };
                DataModel dom6 = new DataModel() { sts = "0" };
                DataModel dom7 = new DataModel() { sts = "1" };
                DataModel dom8 = new DataModel() { sts = "0" };
                DataModel dom9 = new DataModel() { sts = "1" };
                DataModel dom10 = new DataModel() { sts = "0" };
                DataModel dom11 = new DataModel() { sts = "1" };
                DataModel dom12 = new DataModel() { sts = "0" };
                DataModel dom13 = new DataModel() { sts = "1" };
                DataModel dom14 = new DataModel() { sts = "0" };
                DataModel dom15 = new DataModel() { sts = "1" };
                DataModel dom16 = new DataModel() { sts = "0" };
                DataModel dom17 = new DataModel() { sts = "1" };
                DataModel dom18 = new DataModel() { sts = "0" };
                DataSILOM ds = new DataSILOM();
                ds.m01 = dom1;
                ds.m02 = dom2;
                ds.m03 = dom3;
                ds.m04 = dom4;
                ds.m05 = dom5;
                ds.m06 = dom6;
                ds.m07 = dom7;
                ds.m08 = dom8;
                ds.m09 = dom9;
                ds.m10 = dom10;
                ds.m11 = dom11;
                ds.m12 = dom12;
                ds.m13 = dom13;
                ds.m14 = dom14;
                ds.m15 = dom15;
                ds.m16 = dom16;
                ds.m17 = dom17;
                ds.m18 = dom18;

                String Data = joDom.SILOBMToJson(ds);
                bool addFlag1 = dbOperationERP.Db_AddNew("INSERT INTO P_SILO_M_IN(DATA,DATE_IN,STATUS) VALUES('" + Data + "',to_date('" + DateTime.Now.ToString() + "','YYYY-MM-DD HH24:MI:SS'),'0')");
                if (addFlag1)
                {
                    lm.ShowMsg("插入数据库成功。");
                }
                else
                {
                    lm.ShowMsg("插入数据库失败。");
                }
            }
            catch
            {
                lm.ShowMsg("插入数据库-异常！");
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            setERPFASTLSDatabase();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            setMFASTLSDatabase();
        }
        //写入本地数据库SILOM
        private void setMFASTLSDatabase()
        {
            try
            {
                JsonOperation joDom = new JsonOperation();
                DataModel dom1 = new DataModel() { sts = "1" };
                DataModel dom2 = new DataModel() { sts = "0" };
                DataModel dom3 = new DataModel() { sts = "1" };
                DataModel dom4 = new DataModel() { sts = "0" };
                DataModel dom5 = new DataModel() { sts = "1" };
                DataModel dom6 = new DataModel() { sts = "0" };
                DataModel dom7 = new DataModel() { sts = "1" };
                DataModel dom8 = new DataModel() { sts = "0" };
                DataModel dom9 = new DataModel() { sts = "1" };
                DataModel dom10 = new DataModel() { sts = "0" };
                DataModel dom11 = new DataModel() { sts = "1" };
                DataModel dom12 = new DataModel() { sts = "0" };
                DataModel dom13 = new DataModel() { sts = "1" };
                DataModel dom14 = new DataModel() { sts = "0" };
                DataModel dom15 = new DataModel() { sts = "1" };
                DataModel dom16 = new DataModel() { sts = "0" };
                DataModel dom17 = new DataModel() { sts = "1" };
                DataModel dom18 = new DataModel() { sts = "0" };
                DataModel dom19 = new DataModel() { sts = "1" };
                DataModel dom20 = new DataModel() { sts = "0" };
                DataModel dom21 = new DataModel() { sts = "1" };
                DataModel dom22 = new DataModel() { sts = "0" };
                DataModel dom23 = new DataModel() { sts = "1" };
                DataModel dom24 = new DataModel() { sts = "0" };
                DataModel dom25 = new DataModel() { sts = "1" };
                DataModel dom26 = new DataModel() { sts = "0" };
                DataModel dom27 = new DataModel() { sts = "1" };
                DataModel dom28 = new DataModel() { sts = "0" };
                DataModel dom29 = new DataModel() { sts = "1" };
                DataModel dom30 = new DataModel() { sts = "0" };
                DataModel dom31 = new DataModel() { sts = "1" };
                DataModel dom32 = new DataModel() { sts = "0" };
                DataFastLsModel ds = new DataFastLsModel();
                ds.k01 = dom1;
                ds.k02 = dom2;
                ds.k03 = dom3;
                ds.k04 = dom4;
                ds.k05 = dom5;
                ds.k06 = dom6;
                ds.k07 = dom7;
                ds.k08 = dom8;
                ds.k09 = dom9;
                ds.k10 = dom10;
                ds.k11 = dom11;
                ds.k12 = dom12;
                ds.k13 = dom13;
                ds.k14 = dom14;
                ds.k15 = dom15;
                ds.k16 = dom16;
                ds.k17 = dom17;
                ds.k18 = dom18;
                ds.k19 = dom19;
                ds.k20 = dom20;
                ds.k21 = dom21;
                ds.k22 = dom22;
                ds.k23 = dom23;
                ds.k24 = dom24;
                ds.k25 = dom25;
                ds.k26 = dom26;
                ds.k27 = dom27;
                ds.k28 = dom28;
                ds.k29 = dom29;
                ds.k30 = dom30;
                ds.k31 = dom31;
                ds.k32 = dom32;

                String Data = joDom.FastLsToJson(ds);

                bool addFlag1 = dbOperationLocalHost.Db_AddNew("INSERT INTO P_FAST_LS_OUT(DATA,DATE_IN,STATUS) VALUES('" + Data + "',to_date('" + DateTime.Now.ToString() + "','YYYY-MM-DD HH24:MI:SS'),'0')");
                if (addFlag1)
                {
                    lm.ShowMsg("插入数据库成功。");
                }
                else
                {
                    lm.ShowMsg("插入数据库失败。");
                }
            }
            catch
            {
                lm.ShowMsg("插入数据库-异常！");
            }
        }
        //写入远程ERP数据库SILOm
        private void setERPFASTLSDatabase()
        {
            try
            {
                JsonOperation joDom = new JsonOperation();
                DataFastLsInModel dflm1 = new DataFastLsInModel();
                dflm1.pn = "abc";
                dflm1.tn = "50";
                dflm1.sts = "1";
                DataFastLsInModel dflm2 = new DataFastLsInModel();
                dflm2.pn = "abc";
                dflm2.tn = "50";
                dflm2.sts = "1";
                DataFastLsInModel dflm3 = new DataFastLsInModel();
                dflm3.pn = "abc";
                dflm3.tn = "50";
                dflm3.sts = "1";
                DataFastLsInModel dflm4 = new DataFastLsInModel();
                dflm4.pn = "abc";
                dflm4.tn = "50";
                dflm4.sts = "1";
                DataFastLsInModel dflm5 = new DataFastLsInModel();
                dflm5.pn = "abc";
                dflm5.tn = "50";
                dflm5.sts = "1";
                DataFastLsInModel dflm6 = new DataFastLsInModel();
                dflm6.pn = "abc";
                dflm6.tn = "50";
                dflm6.sts = "1";
                DataFastLsInModel dflm7 = new DataFastLsInModel();
                dflm7.pn = "abc";
                dflm7.tn = "50";
                dflm7.sts = "1";
                DataFastLsInModel dflm8 = new DataFastLsInModel();
                dflm8.pn = "abc";
                dflm8.tn = "50";
                dflm8.sts = "1";
                DataFastLsInModel dflm9 = new DataFastLsInModel();
                dflm9.pn = "abc";
                dflm9.tn = "50";
                dflm9.sts = "1";
                DataFastLsInModel dflm10 = new DataFastLsInModel();
                dflm10.pn = "abc";
                dflm10.tn = "50";
                dflm10.sts = "1";
                DataFastLsInModel dflm11 = new DataFastLsInModel();
                dflm11.pn = "abc";
                dflm11.tn = "50";
                dflm11.sts = "1";
                DataFastLsInModel dflm12 = new DataFastLsInModel();
                dflm12.pn = "abc";
                dflm12.tn = "50";
                dflm12.sts = "1";
                DataFastLsInModel dflm13 = new DataFastLsInModel();
                dflm13.pn = "abc";
                dflm13.tn = "50";
                dflm13.sts = "1";
                DataFastLsInModel dflm14 = new DataFastLsInModel();
                dflm14.pn = "abc";
                dflm14.tn = "50";
                dflm14.sts = "1";
                DataFastLsInModel dflm15 = new DataFastLsInModel();
                dflm15.pn = "abc";
                dflm15.tn = "50";
                dflm15.sts = "1";
                DataFastLsInModel dflm16 = new DataFastLsInModel();
                dflm16.pn = "abc";
                dflm16.tn = "50";
                dflm16.sts = "1";
                DataFastLsInModel dflm17 = new DataFastLsInModel();
                dflm17.pn = "abc";
                dflm17.tn = "50";
                dflm17.sts = "1";
                DataFastLsInModel dflm18 = new DataFastLsInModel();
                dflm18.pn = "abc";
                dflm18.tn = "50";
                dflm18.sts = "1";
                DataFastLsInModel dflm19 = new DataFastLsInModel();
                dflm19.pn = "abc";
                dflm19.tn = "50";
                dflm19.sts = "1";
                DataFastLsInModel dflm20 = new DataFastLsInModel();
                dflm20.pn = "abc";
                dflm20.tn = "50";
                dflm20.sts = "1";
                DataFastLsInModel dflm21 = new DataFastLsInModel();
                dflm21.pn = "abc";
                dflm21.tn = "50";
                dflm21.sts = "1";
                DataFastLsInModel dflm22 = new DataFastLsInModel();
                dflm22.pn = "abc";
                dflm22.tn = "50";
                dflm22.sts = "1";
                DataFastLsInModel dflm23 = new DataFastLsInModel();
                dflm23.pn = "abc";
                dflm23.tn = "50";
                dflm23.sts = "1";
                DataFastLsInModel dflm24 = new DataFastLsInModel();
                dflm24.pn = "abc";
                dflm24.tn = "50";
                dflm24.sts = "1";
                DataFastLsInModel dflm25 = new DataFastLsInModel();
                dflm25.pn = "abc";
                dflm25.tn = "50";
                dflm25.sts = "1";
                DataFastLsInModel dflm26 = new DataFastLsInModel();
                dflm26.pn = "abc";
                dflm26.tn = "50";
                dflm26.sts = "1";
                DataFastLsInModel dflm27 = new DataFastLsInModel();
                dflm27.pn = "abc";
                dflm27.tn = "50";
                dflm27.sts = "1";
                DataFastLsInModel dflm28 = new DataFastLsInModel();
                dflm28.pn = "abc";
                dflm28.tn = "50";
                dflm28.sts = "1";
                DataFastLsInModel dflm29 = new DataFastLsInModel();
                dflm29.pn = "abc";
                dflm29.tn = "50";
                dflm29.sts = "1";
                DataFastLsInModel dflm30 = new DataFastLsInModel();
                dflm30.pn = "abc";
                dflm30.tn = "50";
                dflm30.sts = "1";
                DataFastLsInModel dflm31 = new DataFastLsInModel();
                dflm31.pn = "abc";
                dflm31.tn = "50";
                dflm31.sts = "1";
                DataFastLsInModel dflm32 = new DataFastLsInModel();
                dflm32.pn = "abc";
                dflm32.tn = "50";
                dflm32.sts = "1";
                DataFastLsToInModel ds = new DataFastLsToInModel();
                ds.k01 = dflm1;
                ds.k02 = dflm2;
                ds.k03 = dflm3;
                ds.k04 = dflm4;
                ds.k05 = dflm5;
                ds.k06 = dflm6;
                ds.k07 = dflm7;
                ds.k08 = dflm8;
                ds.k09 = dflm9;
                ds.k10 = dflm10;
                ds.k11 = dflm11;
                ds.k12 = dflm12;
                ds.k13 = dflm13;
                ds.k14 = dflm14;
                ds.k15 = dflm15;
                ds.k16 = dflm16;
                ds.k17 = dflm17;
                ds.k18 = dflm18;
                ds.k19 = dflm19;
                ds.k20 = dflm20;
                ds.k21 = dflm21;
                ds.k22 = dflm22;
                ds.k23 = dflm23;
                ds.k24 = dflm24;
                ds.k25 = dflm25;
                ds.k26 = dflm26;
                ds.k27 = dflm27;
                ds.k28 = dflm28;
                ds.k29 = dflm29;
                ds.k30 = dflm30;
                ds.k31 = dflm31;
                ds.k32 = dflm32;
                String Data = joDom.FastLsInToJson(ds);
                bool addFlag1 = dbOperationERP.Db_AddNew("INSERT INTO P_FAST_LS_IN(DATA,DATE_IN,STATUS) VALUES('" + Data + "',to_date('" + DateTime.Now.ToString() + "','YYYY-MM-DD HH24:MI:SS'),'0')");
                if (addFlag1)
                {
                    lm.ShowMsg("插入数据库成功。");
                }
                else
                {
                    lm.ShowMsg("插入数据库失败。");
                }
            }
            catch
            {
                lm.ShowMsg("插入数据库-异常！");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked == true)
            {
                strCheckBox1 = "1";
            }
            else
            {
                strCheckBox1 = "0";
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.Checked == true)
            {
                strCheckBox2 = "1";
            }
            else
            {
                strCheckBox2 = "0";
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox3.Checked == true)
            {
                strCheckBox3 = "1";
            }
            else
            {
                strCheckBox3 = "0";
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox4.Checked == true)
            {
                strCheckBox4 = "1";
            }
            else
            {
                strCheckBox4 = "0";
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ClearTableModel ctm = ct.getClearTable();
            // 得到 hour minute second  如果等于某个值就开始执行某个程序。
            DateTime dt = DateTime.Now;
            int intHour = dt.Hour;
            int intMinute = dt.Minute;
            //定制时间； 比如 在10：30 ：00 的时候执行某个函数
            int iHour = 14;
            int iMinute = 45;

            // 设置　定制时间开始执行程序
            if (intHour == iHour && intMinute == iMinute)
            {

            }
            ct.ClearSiloBOutTable("SILOBOUT", ctm.Siloboutday,dbOperationLocalHost);
        }
    }
}
