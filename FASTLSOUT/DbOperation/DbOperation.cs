using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;//
using System.Windows.Forms;
using System.IO;
using System.Data;
using Oracle.DataAccess.Client;
namespace EltApplication
{
    class dbOperation
    {
        string connStrL2;
        /// <summary>
        /// 构造函数
        /// </summary>
        public dbOperation()
        {

        }
        public dbOperation(string xmlName)
        {

            string[] str = new string[8];
            int i = 0;
            XmlTextReader Reader = new XmlTextReader(Application.StartupPath + "\\" + xmlName + ".xml");
            try
            {

                while (Reader.Read())
                {
                    if (Reader.NodeType == XmlNodeType.Text)
                    {
                        str[i] = Reader.Value;
                        i++;
                        Reader.MoveToNextAttribute();
                    }
                }
                if (str[3].Trim() == "null")
                {
                    str[3] = "";
                }
                connStrL2 = "data source = " + str[0] + "/" + str[1] + " ; user id = " + str[2] + " ;Password=" + str[3] + ";Max Pool Size=20";

            }
            catch (Exception e)
            {

            }
            finally
            {
                if (Reader != null)
                {
                    Reader.Close();
                }
            }
        }
        //-----------------------------------------------------------------------------
        //关于数据库操作的函数集
        //-----------------------------------------------------------------------------
        /// <summary>
        /// 建立与数据库的连接
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public OracleConnection connection(string connStr)
        {
            OracleConnection Conn = new OracleConnection();
            Conn.ConnectionString = connStr;
            return Conn;
        }
        /// <summary>
        /// 建立与数据库的连接
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public OracleConnection connection()
        {
            OracleConnection Conn = new OracleConnection();
            Conn.ConnectionString = connStrL2;
            return Conn;
        }

        /// <summary>
        /// 无返回sql语句执行方法
        /// </summary>
        /// <param name="Sql"></param>
        /// <returns></returns>
        public bool DB_ExecuteNonQuery(string Sql)
        {
            //无返回sql语句执行方法的定义  
            OracleConnection dbConn = new OracleConnection(connStrL2);
            try
            {

                dbConn.Open();
                OracleCommand cmd = new OracleCommand(Sql, dbConn);
                if (cmd.ExecuteNonQuery() != 0)
                {
                    cmd.Dispose();
                    dbConn.Close();
                    return true;
                }
                else
                {
                    cmd.Dispose();
                    dbConn.Close();
                    return false;
                }
            }
            catch (Exception err)
            {
                return false;

            }
            finally
            {
                if (dbConn != null)
                {
                    dbConn.Close();
                }
            }
        }

        /// <summary>
        /// 查询数据库记录,输出类型为OracleDataReader
        /// </summary>
        /// <param name="Sql">SQL查询语句</param>
        /// <returns>DataTable数据表</returns>

        public OracleDataReader DB_ExecuteReader(string Sql)
        {
            //查询输入方法定义，输出类型为OracleDataReader 
            OracleConnection dbConn = new OracleConnection(connStrL2);
            try
            {

                dbConn.Open();
                OracleCommand cmd = new OracleCommand(Sql, dbConn);
                OracleDataReader Dr = cmd.ExecuteReader();
                return Dr;
            }
            catch (Exception e)
            {

                if (dbConn != null)
                    dbConn.Close();
                throw e;
            }
            finally
            {
                if (dbConn != null)
                {
                    dbConn.Close();
                }
            }

        }

        /// <summary>
        ///  查询数据库记录,返回存放记录的DataTable
        /// </summary>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="Sql">SQL查询语句</param>
        /// <returns>DataTable数据表</returns>
        public DataTable DB_Find(string Sql)
        {
            OracleConnection dbConn = new OracleConnection(connStrL2);
            DataTable dt = new DataTable();
            try
            {
                //dbConn.Open();
                OracleDataAdapter Sda = new OracleDataAdapter(Sql, dbConn);

                Sda.Fill(dt);
                dbConn.Close();
                return dt;
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                if (dbConn != null)
                {
                    dbConn.Close();
                }
            }

        }


        /// <summary>
        /// 查找数据表中是否存在某个记录
        /// </summary>
        /// <param name="Sql">SQL查询语句</param>
        /// <returns>整形变量，0－没有符合记录；大于0－找到符合记录</returns>
        public int IsRecorderExist(string Sql)
        {
            OracleConnection dbConn = new OracleConnection(connStrL2);
            try
            {
                dbConn.Open();
                OracleDataAdapter Sda = new OracleDataAdapter(Sql, dbConn);
                DataTable dt = new DataTable();
                Sda.Fill(dt);
                dbConn.Close();
                return dt.Rows.Count;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (dbConn != null)
                {
                    dbConn.Close();
                }
            }
        }

        /// <summary>
        /// 在对应的数据表里添加新记录
        /// </summary>
        ///<param name="connStr">数据库连接字符串</param>
        ///<param name="strTableName">需要添加记录的数据表</param>
        /// <param name="dt">需要添加记录的数据表所暂存的DataTable</param>
        /// <param name="strValues">新记录的各字段值组成的字符串数组</param>
        public bool Db_AddNew(string strSql)
        {
            OracleConnection dbConn = new OracleConnection(connStrL2);
            try
            {
                OracleCommand cmdAddNew = new OracleCommand(strSql, dbConn);
                dbConn.Open();
                OracleDataReader Sdr = cmdAddNew.ExecuteReader();
                Sdr.Close();
                dbConn.Close();


                return true;
            }
            catch (Exception ex)
            {
                if (dbConn != null)
                {
                    dbConn.Close();
                }
                return false;
            }
            finally
            {
                if (dbConn != null)
                {
                    dbConn.Close();
                }
            }
        }


        /// <summary>
        /// 在对应的数据表里添加新记录
        /// </summary>
        ///<param name="connStr">数据库连接字符串</param>
        ///<param name="strTableName">需要添加记录的数据表</param>
        /// <param name="dt">需要添加记录的数据表所暂存的DataTable</param>
        /// <param name="strValues">新记录的各字段值组成的字符串数组</param>
        public bool Db_AddNew(string strTableName, string[] dt, string[] strValues)
        {
            OracleConnection dbConn = new OracleConnection(connStrL2);
            try
            {
                string strSql = "", strField = "", strValue = "";
                for (int i = 0; i < dt.Length; i++)
                {
                    if (strValues[i] == "")
                    {
                        strField += dt[i] + ",";
                        strValue += "null,";
                    }
                    else
                    {
                        strField += dt[i] + ",";
                        strValue += "'" + strValues[i] + "',";
                    }

                }
                int nPos = strField.LastIndexOf(@",");
                strField = strField.Substring(0, nPos);
                nPos = strValue.LastIndexOf(@",");
                strValue = strValue.Substring(0, nPos);
                strSql = String.Format("INSERT INTO {0}({1}) VALUES({2})", strTableName, strField, strValue);


                OracleCommand cmdAddNew = new OracleCommand(strSql, dbConn);
                dbConn.Open();
                OracleDataReader Sdr = cmdAddNew.ExecuteReader();
                Sdr.Close();
                dbConn.Close();


                return true;
            }
            catch (Exception ex)
            {
                if (dbConn != null)
                {
                    dbConn.Close();
                }
                return false;
            }
            finally
            {
                if (dbConn != null)
                {
                    dbConn.Close();
                }
            }
        }

        /// <summary>
        /// 在对应的数据表里添加新记录
        /// </summary>
        ///<param name="connStr">数据库连接字符串</param>
        ///<param name="strTableName">需要添加记录的数据表</param>
        /// <param name="dt">需要添加记录的数据表所暂存的DataTable</param>
        /// <param name="strValues">新记录的各字段值组成的字符串数组</param>
        public bool Db_AddNew(string strTableName, string[] dt, string[] strValues, int[] dateColumnIndex)
        {
            OracleConnection dbConn = new OracleConnection(connStrL2);
            try
            {
                string strSql = "", strField = "", strValue = "";
                for (int i = 0; i < dt.Length; i++)
                {
                    if (strValues[i] == "")
                    {
                        strField += dt[i] + ",";
                        strValue += "null,";
                    }
                    else
                    {
                        strField += dt[i] + ",";
                        //strValue += "'" + strValues[i] + "',";

                        //判断字段是否是dateColumnIndex数组中所列的字段
                        int isExist = Array.IndexOf(dateColumnIndex, i);
                        //如果是日期字段，加日期转化
                        if (isExist != -1)
                        {
                            strValue += "TO_DATE('" + strValues[i] + "','yyyy-mm-dd hh24:mi:ss'),";
                        }
                        else
                        {
                            strValue += "'" + strValues[i] + "',";
                        }
                    }

                }
                int nPos = strField.LastIndexOf(@",");
                strField = strField.Substring(0, nPos);
                nPos = strValue.LastIndexOf(@",");
                strValue = strValue.Substring(0, nPos);
                strSql = String.Format("INSERT INTO {0}({1}) VALUES({2})", strTableName, strField, strValue);


                OracleCommand cmdAddNew = new OracleCommand(strSql, dbConn);
                dbConn.Open();
                OracleDataReader Sdr = cmdAddNew.ExecuteReader();
                Sdr.Close();
                dbConn.Close();


                return true;
            }
            catch (Exception ex)
            {
                if (dbConn != null)
                {
                    dbConn.Close();
                }
                return false;
            }
            finally
            {
                if (dbConn != null)
                {
                    dbConn.Close();
                }
            }
        }
        /// <summary>
        /// 在对应的数据表里添加新记录
        /// </summary>
        ///<param name="connStr">数据库连接字符串</param>
        ///<param name="strTableName">需要添加记录的数据表</param>
        /// <param name="dt">需要添加记录的数据表所暂存的DataTable</param>
        /// <param name="strValues">新记录的各字段值组成的字符串数组</param>
        /// <param name="dateColumnIndex">整形数组，定义字段中是日期型字段的序号，从0开始</param>
        public bool Db_AddNew(string strTableName, DataTable dt, string[] strValues, int[] dateColumnIndex)
        {
            OracleConnection dbConn = new OracleConnection(connStrL2);
            try
            {
                string[] strDesField = new string[100];
                string strSql = "", strField = "", strValue = "";
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (strValues[i] == "")
                    {

                        strDesField[i] = dt.Columns[i].ColumnName;
                        strField += strDesField[i] + ",";
                        strValue += "null,";
                    }
                    else
                    {
                        strDesField[i] = dt.Columns[i].ColumnName;
                        strField += strDesField[i] + ",";
                        //判断字段是否是dateColumnIndex数组中所列的字段
                        int isExist = Array.IndexOf(dateColumnIndex, i);
                        //如果是日期字段，加日期转化
                        if (isExist != -1)
                        {
                            strValue += "TO_DATE('" + strValues[i] + "','yyyy-mm-dd hh24:mi:ss'),";
                        }
                        else
                        {
                            strValue += "'" + strValues[i] + "',";
                        }

                    }

                }
                int nPos = strField.LastIndexOf(@",");
                strField = strField.Substring(0, nPos);
                nPos = strValue.LastIndexOf(@",");
                strValue = strValue.Substring(0, nPos);
                strSql = String.Format("INSERT INTO {0}({1}) VALUES({2})", strTableName, strField, strValue);
                OracleCommand cmdAddNew = new OracleCommand(strSql, dbConn);
                dbConn.Open();
                OracleDataReader Sdr = cmdAddNew.ExecuteReader();
                Sdr.Close();
                dbConn.Close();
                return true;
            }
            catch (Exception ex)
            {
                if (dbConn != null)
                {
                    dbConn.Close();
                }
                return false;
            }
            finally
            {
                if (dbConn != null)
                {
                    dbConn.Close();
                }
            }
        }


        /// <summary>
        /// 在对应的数据表里删除记录
        /// </summary>
        /// <param name="strTableName">源数据表名</param>
        /// <param name="strKey">数据表主键</param>
        /// <param name="strFilter">主键的匹配值</param>
        public void DB_Delete(string strTableName, string strKey, string strFilter)
        {
            OracleConnection dbConn = new OracleConnection(connStrL2);
            try
            {
                string Sql = String.Format("DELETE FROM {0} WHERE {1}='{2}'", strTableName, strKey, strFilter);
                OracleCommand cmdDel = new OracleCommand(Sql, dbConn);
                dbConn.Open();
                OracleDataReader Sdr = cmdDel.ExecuteReader();
                Sdr.Close();
                dbConn.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (dbConn != null)
                {
                    dbConn.Close();
                }
            }
        }

        public bool DB_Delete(string strDelete)
        {
            OracleConnection dbConn = new OracleConnection(connStrL2);
            try
            {
                string Sql = strDelete;
                OracleCommand cmdDel = new OracleCommand(Sql, dbConn);
                dbConn.Open();
                OracleDataReader Sdr = cmdDel.ExecuteReader();                
                Sdr.Close();
                dbConn.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (dbConn != null)
                {
                    dbConn.Close();
                }
            }
        }


        /// <summary>
        /// 更新数据库中的记录,针对单条记录修改
        /// </summary>
        /// <param name="strSql">更新某条需要修改的记录的SQL语句</param>
        /// <returns>更新成功或失败的标识</returns>
        public bool DB_Update(string Sql)
        {
            OracleConnection dbConn = new OracleConnection(connStrL2);
            try
            {
                dbConn.Open();
                OracleCommand cmdUpdate = dbConn.CreateCommand();
                cmdUpdate.CommandText = Sql;
                cmdUpdate.ExecuteReader();
                dbConn.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (dbConn != null)
                {
                    dbConn.Close();
                }
            }
        }
        /// <summary>
        /// 更新数据库中与参数中的SQL查询符合的记录,针对单条记录修改
        /// </summary>
        /// <param name="strSql">查询某条需要修改的记录的SQL语句</param>
        /// <param name="strValue">各字段的新值，字符串数组</param>
        /// <returns>更新后的数据表DataTable</returns>
        public DataTable DB_Update(string Sql, string[] strValue)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = DB_Find(Sql);
                //DataTable dtNew = new DataTable();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (strValue[i] == "")
                    {
                        dt.Rows[0][dt.Columns[i].ColumnName] = DBNull.Value;
                    }
                    else
                    {
                        dt.Rows[0][dt.Columns[i].ColumnName] = strValue[i];
                    }
                }
                OracleDataAdapter Sda = new OracleDataAdapter(Sql, connStrL2);
                OracleCommandBuilder cmbUpdate = new OracleCommandBuilder(Sda);
                Sda.Update(dt);
                dt.AcceptChanges();
                return dt;
            }
            catch (Exception ex)
            {
                return dt;
            }

        }
        /// <summary>
        /// 更新数据库中与参数中的SQL查询符合的记录,针对单条记录修改
        /// </summary>
        /// <param name="strSql">查询某条需要修改的记录的SQL语句</param>
        /// <param name="strValue">各字段的新值，字符串数组</param>
        /// <returns>更新后的数据表DataTable</returns>
        /// <param name="dateColumnIndex">整形数组，定义要修改的字段中是日期型字段的序号，从0开始</param>
        public DataTable DB_Update(string Sql, string[] strValue, int[] dateColumnIndex)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = DB_Find(Sql);
                //DataTable dtNew = new DataTable();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (strValue[i] == "")
                    {
                        dt.Rows[0][dt.Columns[i].ColumnName] = DBNull.Value;
                    }
                    else
                    {
                        //dt.Rows[0][dt.Columns[i].ColumnName] = strValue[i];

                        //判断字段是否是dateColumnIndex数组中所列的字段
                        int isExist = Array.IndexOf(dateColumnIndex, i);
                        //如果是日期字段，加日期转化
                        if (isExist != -1)
                        {
                            dt.Rows[0][dt.Columns[i].ColumnName] = DateTime.Parse(strValue[i]); //"TO_DATE('" + strValue[i] + "','yyyy-mm-dd hh24:mi:ss'),";
                        }
                        else
                        {
                            dt.Rows[0][dt.Columns[i].ColumnName] = strValue[i];
                        }
                    }
                }
                OracleDataAdapter Sda = new OracleDataAdapter(Sql, connStrL2);
                OracleCommandBuilder cmbUpdate = new OracleCommandBuilder(Sda);
                Sda.Update(dt);
                dt.AcceptChanges();
                return dt;
            }
            catch (Exception ex)
            {
                return dt;
            }
        }
    }

}