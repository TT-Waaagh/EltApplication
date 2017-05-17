using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using EltApplication.Tools;
using System.Runtime.InteropServices;
using Oracle.DataAccess.Client;
using System.IO;


namespace EltApplication
{
    public partial class Form2 : Form
    {
        #region 结构体定义
        public struct CON_TABLE_TYPE //待连接plc地址属性表
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] Adr; // connection address
            public byte AdrType; // Type of address: IP (2)
            public byte SlotNr; // Slot number
            public byte RackNr; // Rack number
        }

        public enum DatType : byte//PLC数据类型
        {
            BYTE = 0x02,
            WORD = 0x04,
            DWORD = 0x06,
        }

        public enum FieldType : byte//PLC区域类型
        {
            //Value types as ASCII characters区域类型对应的ASCII字符
            //data byte (d/D)
            d = 100,
            D = 68,
            //input byte (e/E)
            e = 101,
            E = 69,
            //output byte (a/A)
            a = 97,
            A = 65,
            //memory byte (m/M)
            m = 109,
            M = 77,
            //timer word (t/T),
            t = 116,
            T = 84,
        }
        #endregion

        #region
        [DllImport("Prodave6.dll")]
        public extern static int LoadConnection_ex6(int ConNr, string pAccessPoint, int ConTableLen, ref  CON_TABLE_TYPE pConTable);
        //参数：连接号（0-63）、常值"S7ONLINE"、待连接plc地址属性表长度（字节为单位，常值9）、待连接plc地址属性表
        [DllImport("Prodave6.dll")]
        public extern static int SetActiveConnection_ex6(UInt16 ConNr);
        [DllImport("Prodave6.dll")]
        public extern static int db_read_ex6(UInt16 BlkNr, DatType DType, UInt16 StartNr, ref UInt32 pAmount, UInt32 BufLen, byte[] pBuffer, ref UInt32 pDatLen);
        //参数：data block号、要读取的数据类型、起始地址号、需要读取类型的数量、缓冲区长度（字节为单位）、缓冲区、缓冲区数据交互的长度
        [DllImport("Prodave6.dll")]
        public extern static int db_write_ex6(UInt16 BlkNr, DatType Type, UInt16 StartNr, ref UInt32 pAmount, UInt32 BufLen, byte[] pBuffer);
       
        #endregion

        #region 常值定义（用于极限值）
        public const int MAX_CONNECTIONS = 64; // 64 is default in PRODAVE
        public const int MAX_DEVNAME_LEN = 128;// e.g. "S7ONLINE"
        public const int MAX_BUFFERS = 24; // for blk_read() and blk_write()
        public const int MAX_BUFFER = 65536; // Transfer buffer for error text)
        #endregion

        #region 自定义与PLC的通讯函数

        public static void GetBitFromByte(byte byteData, int bitNo, ref bool bitData)
        {
            if (bitNo >= 0 && bitNo <= 7)
            {
                byte[] byteArray = new byte[1];
                byteArray[0] = byteData;
                System.Collections.BitArray BA = new System.Collections.BitArray(byteArray);
                bitData = BA.Get(bitNo);
            }
        }

        public static string GetErrInfo(int errCode)
        {
            switch (errCode)
            {
                case -1: return "User-Defined  Error!";
                case 0x0000: return "Success";
                case 0x0001: return "Load dll failed";
                case 0x00E1: return "User max";
                case 0x00E2: return "SCP entry";
                case 0x00E7: return "SCP board open";
                case 0x00E9: return "No Windows server";
                case 0x00EA: return "Protect";
                case 0x00CA: return "SCP no resources";
                case 0x00CB: return "SCP configuration";
                case 0x00CD: return "SCP illegal";
                case 0x00CE: return "SCP incorrect parameter";
                case 0x00CF: return "SCP open device";
                case 0x00D0: return "SCP board";
                case 0x00D1: return "SCP software";
                case 0x00D2: return "SCP memory";
                case 0x00D7: return "SCP no meas";
                case 0x00D8: return "SCP user mem";
                case 0x00DB: return "SCP timeout";
                case 0x00F0: return "SCP db file does not exist";
                case 0x00F1: return "SCP no global dos memory";
                case 0x00F2: return "SCP send not successful";
                case 0x00F3: return "SCP receive not successful";
                case 0x00F4: return "SCP no device available";
                case 0x00F5: return "SCP illegal subsystem";
                case 0x00F6: return "SCP illegal opcode";
                case 0x00F7: return "SCP buffer too short";
                case 0x00F8: return "SCP buffer1 too short";
                case 0x00F9: return "SCP illegal protocol sequence";
                case 0x00FA: return "SCP illegal PDU arrived";
                case 0x00FB: return "SCP request error";
                case 0x00FC: return "SCP no license";
                case 0x0101: return "Connection is not established / parameterized";
                case 0x010a: return "Negative Acknowledgment received / timeout errors";
                case 0x010c: return "Data not available or locked";
                case 0x012A: return "No system memory left";
                case 0x012E: return "Incorrect parameter";
                case 0x0132: return "No storage space in the DPRAM";
                case 0x0200: return "xx";
                case 0x0201: return "Falsche Schnittstelle angegeben";
                case 0x0202: return "Incorrect interface indicated";
                case 0x0203: return "Toolbox already installed";
                case 0x0204: return "Toolbox with other compounds already installed";
                case 0x0205: return "Toolbox is not installed";
                case 0x0206: return "Handle can not be set";
                case 0x0207: return "Data segment can not be blocked";
                case 0x0209: return "Erroneous data field";
                case 0x0300: return "Timer init error";
                case 0x0301: return "Com init error";
                case 0x0302: return "Module is too small, DW does not exist";
                case 0x0303: return "Block boundary erschritten, number correct";
                case 0x0310: return "Could not find any hardware";
                case 0x0311: return "Hardware defective";
                case 0x0312: return "Incorrect configuration parameters";
                case 0x0313: return "Incorrect baud rate/interrupt vector";
                case 0x0314: return "HSA incorrectly parameterized";
                case 0x0315: return "Address already assigned";
                case 0x0316: return "Device already assigned";
                case 0x0317: return "Interrupt not available";
                case 0x0318: return "Interrupt occupied";
                case 0x0319: return "SAP not occupied";
                case 0x031A: return "Could not find any remote station";
                case 0x031B: return "syni error";
                case 0x031C: return "System error";
                case 0x031D: return "Error in buffer size";
                case 0x0320: return "DLL/VxD not found";
                case 0x0321: return "DLL function error";
                case 0x0330: return "Version conflict";
                case 0x0331: return "Com config error";
                case 0x0332: return "smc timeout";
                case 0x0333: return "Com not configured";
                case 0x0334: return "Com not available";
                case 0x0335: return "Serial drive in use";
                case 0x0336: return "No connection";
                case 0x0337: return "Job rejected";
                case 0x0380: return "Internal error";
                case 0x0381: return "Device not in Registry";
                case 0x0382: return "L2 driver not in Registry";
                case 0x0384: return "L4 driver not in Registry";
                case 0x03FF: return "System error";
                case 0x4001: return "Connection is not known";
                case 0x4002: return "Connection is not established";
                case 0x4003: return "Connection is being established";
                case 0x4004: return "Connection is collapsed";
                case 0x0800: return "Toolbox occupied";
                case 0x8001: return "in this mode is not allowed";
                case 0x8101: return "Hardware error";
                case 0x8103: return "Object Access not allowed";
                case 0x8104: return "Context is not supported";
                case 0x8105: return "ungtige Address";
                case 0x8106: return "Type (data) is not supported";
                case 0x8107: return "Type (data) is not consistent";
                case 0x810A: return "Object does not exist";
                case 0x8301: return "Memory on CPU is not sufficient";
                case 0x8404: return "grave error";
                case 0x8500: return "Incorrect PDU Size";
                case 0x8702: return "Invalid address";
                case 0xA0CE: return "User occupied";
                case 0xA0CF: return "User does not pick up";
                case 0xA0D4: return "Connection not available because modem prevents immediate redial (waiting time before repeat dial not kept to) ";
                case 0xA0D5: return "No dial tone";
                case 0xD201: return "Syntax error module name";
                case 0xD202: return "Syntax error function parameter";
                case 0xD203: return "Syntax error Bausteshortyp";
                case 0xD204: return "no memory module in eingeketteter";
                case 0xD205: return "Object already exists";
                case 0xD206: return "Object already exists";
                case 0xD207: return "Module available in the EPROM";
                case 0xD209: return "Module does not exist";
                case 0xD20E: return "no module present";
                case 0xD210: return "Block number is too big";
                case 0xD241: return "Protection level of function is not sufficient";
                case 0xD406: return "Information not available";
                case 0xEF01: return "Wrong ID2";
                case 0xFFFE: return "unknown error FFFE hex";
                case 0xFFFF: return "Timeout error. Interface KVD";
                default: return "unknown error";
            }
        }
        #endregion  

        UInt16 BlkNr;
        UInt16 StartNr;
        UInt32 pAmount;
        UInt32 BufLen;
        byte[] pReadBuffer = new byte[3];
        byte[] pWriteBuffer = new byte[3];
        UInt32 pDatLen;

        int RetValue;
        int RetValue1;
        string[] runStatus = new string[MAX_BUFFERS];
        DataModel dom1 = new DataModel();
        DataModel dom2 = new DataModel();
        DataModel dom3 = new DataModel();
        DataModel dom4 = new DataModel();
        DataModel dom5 = new DataModel();
        DataModel dom6 = new DataModel();
        DataModel dom7 = new DataModel();
        DataModel dom8 = new DataModel();
        bool rt = false;
   
        LogMsg lm = new LogMsg("D:\\EltApplication\\SiementsLog.txt");
        dbOperation dbOperationERP = new dbOperation("ERPDBconfig");
        dbOperation dbOperationLocalHost = new dbOperation("DBconfig");

        public Form2()
        {
            InitializeComponent();

            //PLC连接参数设置
            short ConNr = 1;  // 连接号
            string AccessPoint = "S7ONLINE";
            CON_TABLE_TYPE ConTable;  // 属性表
            int ConTableLen = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CON_TABLE_TYPE)); //属性表长度（字节为单位)
            ConTable.AdrType = 2; // 通讯方式
            ConTable.RackNr = 0;  // 机架号 
            ConTable.SlotNr = 3;  // CPU槽号
            ConTable.Adr = new byte[] { 192, 168, 1, 88, 0, 0 };  // PLC的IP地址

            try
            {
                RetValue = LoadConnection_ex6(ConNr, AccessPoint, ConTableLen, ref ConTable);
            }
            catch (Exception ex) {
             //   WriteLog(ex, "链接PLC！", "");
                lm.ShowMsg("链接PLC通讯失败！");
            }
       }

        public static object locker = new object();

        private void ReadPLCData()
        {
            BlkNr = 1;    //data block号
            StartNr = 0;  //起始地址
            pAmount = 8; //需要读取类型的数量
            BufLen = 8;   //缓冲区长度（字节为单位）
            pDatLen = 0;
            int i = 0;
            int j = 0;
            int bitNo = 0;

            try
            {
                RetValue1 = db_read_ex6(BlkNr, DatType.BYTE, StartNr, ref pAmount, BufLen, pReadBuffer, ref  pDatLen);
                if (RetValue1 != 0)
                {
                 //   MessageBox.Show("参数有误，读取PLC数据失败!");
                    lm.ShowMsg("读取PLC数据失败!");
                }
                else
                {
                    for (i = 0; i < pAmount; i++)
                    {
                        if (bitNo > 7)
                        {
                            bitNo = 0;
                            j++;
                        }
                        GetBitFromByte(pReadBuffer[j], bitNo, ref rt);
                        bitNo++;
                        runStatus[i] = (string)rt.ToString();
                        if (runStatus[i] == "True")
                        {
                            switch (i)
                            {
                                case 0:
                                    dom1.sts = "1";
                                    textBox1.BackColor = Color.YellowGreen;
                                    textBox1.Text = "运行";
                                    break;
                                case 1:
                                    dom2.sts = "1";
                                    textBox2.BackColor = Color.YellowGreen;
                                    textBox2.Text = "运行";
                                    break;
                                case 2:
                                    dom3.sts = "1";
                                    textBox3.BackColor = Color.YellowGreen;
                                    textBox3.Text = "运行";
                                    break;
                                case 3:
                                    dom4.sts = "1";
                                    textBox4.BackColor = Color.YellowGreen;
                                    textBox4.Text = "运行";
                                    break;
                                case 4:
                                    dom5.sts = "1";
                                    textBox5.BackColor = Color.YellowGreen;
                                    textBox5.Text = "运行";
                                    break;
                                case 5:
                                    dom6.sts = "1";
                                    textBox6.BackColor = Color.YellowGreen;
                                    textBox6.Text = "运行";
                                    break;
                                case 6:
                                    dom7.sts = "1";
                                    textBox7.BackColor = Color.YellowGreen;
                                    textBox7.Text = "运行";
                                    break;
                                case 7:
                                    dom8.sts = "1";
                                    textBox8.BackColor = Color.YellowGreen;
                                    textBox8.Text = "运行";
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            switch (i)
                            {
                                case 0:
                                    dom1.sts = "0";
                                    textBox1.BackColor = Color.Red;
                                    textBox1.Text = "停止";
                                    break;
                                case 1:
                                    dom2.sts = "0";
                                    textBox2.BackColor = Color.Red;
                                    textBox2.Text = "停止";
                                    break;
                                case 2:
                                    dom3.sts = "0";
                                    textBox3.BackColor = Color.Red;
                                    textBox3.Text = "停止";
                                    break;
                                case 3:
                                    dom4.sts = "0";
                                    textBox4.BackColor = Color.Red;
                                    textBox4.Text = "停止";
                                    break;
                                case 4:
                                    dom5.sts = "0";
                                    textBox5.BackColor = Color.Red;
                                    textBox5.Text = "停止";
                                    break;
                                case 5:
                                    dom6.sts = "0";
                                    textBox6.BackColor = Color.Red;
                                    textBox6.Text = "停止";
                                    break;
                                case 6:
                                    dom7.sts = "0";
                                    textBox7.BackColor = Color.Red;
                                    textBox7.Text = "停止";
                                    break;
                                case 7:
                                    dom8.sts = "0";
                                    textBox8.BackColor = Color.Red;
                                    textBox8.Text = "停止";
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               lm.ShowMsg("读取PLC数据异常！");
            }

        }

        private void WritePLCData(DataSILOB ds)
        {
            BlkNr = 2;    //data block号
            StartNr = 0;  //起始地址
            pAmount = 1;  //需要写入类型的数量
            BufLen = 1;   //缓冲区长度（字节为单位）
            pDatLen = 0;
                            
            string str1 = ds.b01.sts;
            if (str1 == "1")
            {
                pWriteBuffer[0] = set_bit(pWriteBuffer[0], 1, true);
                textBox11.BackColor = Color.YellowGreen;
                textBox11.Text = "启动"; 
            }
            else if (str1 == "n")
            {
                textBox11.BackColor = Color.DimGray;
                textBox11.Text = "无操作";
            }
            else 
            {
                pWriteBuffer[0] = set_bit(pWriteBuffer[0], 1, false);
                textBox11.BackColor = Color.Red;
                textBox11.Text = "停止";
            }

           
            string str2 = ds.b02.sts;
            if (str2 == "1")
            {
                pWriteBuffer[0] = set_bit(pWriteBuffer[0], 2, true);
                textBox12.BackColor = Color.YellowGreen;
                textBox12.Text = "启动"; 
            }           
            else if (str2 == "n")
            {
                textBox12.BackColor = Color.DimGray;
                textBox12.Text = "无操作";
            }
            else
            {
                pWriteBuffer[0] = set_bit(pWriteBuffer[0], 2, false);
                textBox12.BackColor = Color.Red;
                textBox12.Text = "停止";
            }

            
            string str3 = ds.b03.sts;
            if (str3 == "1")
            {
                pWriteBuffer[0] = set_bit(pWriteBuffer[0], 3, true);
                textBox13.BackColor = Color.YellowGreen;
                textBox13.Text = "启动"; 
            }
            else if (str3 == "n")
            {
                textBox13.BackColor = Color.DimGray;
                textBox13.Text = "无操作";
            }   
            else
            {
                pWriteBuffer[0] = set_bit(pWriteBuffer[0], 3, false);
                textBox13.BackColor = Color.Red;
                textBox13.Text = "停止";
            }

            string str4 = ds.b04.sts;
            if (str4 == "1")
            {
                pWriteBuffer[0] = set_bit(pWriteBuffer[0], 4, true);
                textBox14.BackColor = Color.YellowGreen;
                textBox14.Text = "启动"; 
            }
            else if (str4== "n")
            {
                textBox14.BackColor = Color.Yellow;
                textBox14.Text = "无操作";
            }    
            else
            {
                pWriteBuffer[0] = set_bit(pWriteBuffer[0], 4, false);
                textBox14.BackColor = Color.Red;
                textBox14.Text = "停止";
            }

           try
           {
                RetValue1 = db_write_ex6(BlkNr, DatType.BYTE, StartNr, ref pAmount, BufLen, pWriteBuffer);
                if (RetValue1 != 0)
                {
                   lm.ShowMsg("参数有误，写入PLC数据失败!");
                }
            }
            catch (Exception ex)
            {
               lm.ShowMsg("写入PLC数据异常！");
            }

        }

        // 设置某一位的值
        byte set_bit(byte data, int index, bool flag)
        {
            if (index > 8 || index < 1)
                throw new ArgumentOutOfRangeException();
            int v = index < 2 ? index : (2 << (index - 2));
            return flag ? (byte)(data | v) : (byte)(data & ~v);
        }

        
        //获取PLC数据，测试用，正式运行这段代码加到Timer里
        private void button5_Click_1(object sender, EventArgs e)
        {
            try
            {
                ReadPLCData();
                setMSILOBDatabase(); 

                string yy = DateTime.Now.ToString("yyyy-MM-dd");
                string hh = DateTime.Now.ToString("hh:mm:ss");
                label22.Text = "获取PLC数据成功并已写入本地数据库：" + yy + " " + hh;
            }
            catch (Exception ex)
            {
                lm.ShowMsg(ex.ToString());
            }

         }

        //写入本地数据库SILOB
        private void setMSILOBDatabase()
        {
            try
            {
                JsonOperation joDom = new JsonOperation();
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

        //读取传入本地数据库的ERP数据，测试用，正式运行这段代码加到Timer里
        private void button6_Click_1(object sender, EventArgs e)
        {
            getLocalDatabase();
                        
            string yy = DateTime.Now.ToString("yyyy-MM-dd");
            string hh = DateTime.Now.ToString("hh:mm:ss");
            label21.Text = "接收ERP数据并已写入PLC成功：" + yy + " " + hh;
        }

        //读取本地数据库
        private void getLocalDatabase()
        {
            try
            {
                DataTable dt = new DataTable();
            //    String strSelect = "SELECT * from P_SILO_B_IN WHERE STATUS = '0' order by DATE_IN desc";
                String strSelect = "select * from(select rownum rn,P_SILO_B_IN.* from P_SILO_B_IN) where rn=(select count(*) from P_SILO_B_IN)";
                dt = dbOperationLocalHost.DB_Find(strSelect);
                InOutDataBasicModel model = new InOutDataBasicModel();

                if (dt == null)
                {
                    lm.ShowMsg("取数错误！请检查本地数据库服务！");
                }
                else
                {
                    if (dt.Rows.Count > 0)
                    {
                     //   for (int i = 0; i < dt.Rows.Count; i++)
                    //    {
                            model.Erpid = dt.Rows[0]["ID"].ToString();
                            model.Data = dt.Rows[0]["DATA"].ToString();
                            DataOperations(model.Data);
                            model.Datein = dt.Rows[0]["DATE_IN"].ToString();
                            model.Dateout = dt.Rows[0]["DATE_OUT"].ToString();
                            model.Status = dt.Rows[0]["STATUS"].ToString();

                            UpdateDatabaseStatus(model.Erpid);
                     //   }
                    }

                }
            }
            catch
            {
                lm.ShowMsg("读取数据库异常！");
            }
        }

        //处理本地数据库数据，写入PLC
        private void DataOperations(String strData)
        {
            JsonOperation jo = new JsonOperation();
            DataSILOB ds = jo.SILOBToModel(strData);

            WritePLCData(ds);
        }

        //写入PLC成功，更新本地数据库状态 STATUS=1
        private void UpdateDatabaseStatus(String strId)
        {
            try
            {
                String strUpdataSql = "UPDATE P_SILO_B_IN SET DATE_OUT = to_date('" + DateTime.Now.ToString() + "','YYYY-MM-DD HH24:MI:SS'),STATUS='1' WHERE ERPID='" + strId + "'";
                bool addFlag = dbOperationLocalHost.DB_Update(strUpdataSql);
                if (addFlag)
                {
                    lm.ShowMsg("更改数据库成功！");
                }
                else
                {
                    lm.ShowMsg("更改数据库失败！");
                }
            }
            catch
            {
                lm.ShowMsg("读取数据库-异常！");
            }
        }

        // 接收ERP数据
        private void timer1_Tick(object sender, EventArgs e)
        {
            getLocalDatabase();

            string yy = DateTime.Now.ToString("yyyy-MM-dd");
            string hh = DateTime.Now.ToString("hh:mm:ss");
            label21.Text = "接收ERP数据并已写入PLC成功：" + yy + " " + hh;
         }

        // 获取PLC数据
        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                ReadPLCData();
                setMSILOBDatabase();

                string yy = DateTime.Now.ToString("yyyy-MM-dd");
                string hh = DateTime.Now.ToString("hh:mm:ss");
                label22.Text = "获取PLC数据成功并已写入本地数据库：" + yy + " " + hh;
            }
            catch (Exception ex)
            {
                String aa = ex.ToString();
            }
        }

 

   
    }

}
