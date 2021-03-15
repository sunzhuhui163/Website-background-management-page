using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace NeddleFinishGood.Models
{
    public class SqlHelper
    {
        public SqlHelper()
        {
        }
        public SqlHelper(string connectionStr)
        {
            connStr = connectionStr;
        }
        private string connStr;
        public string ConnStr
        {
            get
            {
                if (connStr == null) connStr = connStrFromXML();
                return connStr;
            }
        }

        private SqlConnection connect = null;

        public SqlConnection getConnect()
        {
            if (string.IsNullOrEmpty(connStr))
            {
                connStr = connStrFromXML();
            }
            //  if (connect == null || connect.State == ConnectionState.Closed)
            connect = new SqlConnection(connStr);
            return connect;
        }

        //输入连接信息
        private string connStrFromXML()
        {
            string connStr = "";
            try
            {
                connStr = ConfigurationManager.ConnectionStrings["connection"].ConnectionString;
            }
            catch (Exception e)
            {
                //throw e;
            }
            return connStr;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns>返回受影响的行数</returns>
        public int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
           int result = -1;
            try
            {
                using (SqlConnection con = getConnect())
                {
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }
                        if (parameters.Count() > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        result = cmd.ExecuteNonQuery();
                    }
               }
            }
            catch
            {

            }

            return result;
        }

        //public DataTable ExecuteDataTable(string sql, params SqlParameter[] parameter)
        //{
        //    using (SqlConnection con = getConnect())
        //    {
        //        using (SqlDataAdapter da = new SqlDataAdapter())
        //        {
        //            da.SelectCommand = new SqlCommand(sql, con);

        //            if (con.State == ConnectionState.Closed)
        //            {
        //                con.Open();
        //            }
        //            if (parameter.Count() > 0)
        //            {
        //                da.SelectCommand.Parameters.AddRange(parameter);
        //            }
        //            DataSet ds = new DataSet();
        //            da.Fill(ds);
        //            if (ds.Tables.Count > 0)
        //                return ds.Tables[0];
        //            else
        //                return null;
        //        }
        //    }
        //}
        //public SqlDataReader ExecuteReader(string sql)
        //{
        //    SqlConnection con = getConnect();

        //    using (SqlCommand cmd = new SqlCommand(sql, con))
        //    {
        //        if (con.State == ConnectionState.Closed)
        //        {
        //            con.Open();
        //        }
        //        return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        //    }

        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns>返回结果集中第一行第一列</returns>
        public object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection con = getConnect())
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        if (parameters.Count() > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch
            {
                return -1;
            }

        }
        public DataSet ExecuteDataSet(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection con = getConnect())
            {
                using (SqlDataAdapter da = new SqlDataAdapter())
                {
                    da.SelectCommand = new SqlCommand(sql, con);

                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    if (parameters.Count() > 0)
                    {
                        da.SelectCommand.Parameters.AddRange(parameters);
                    }
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    return ds;
                }
            }
        }
        //事务中存储与读取
        //public DataTable GetDataTableInTrans(string SqlString, SqlConnection Conn, SqlTransaction SqlConTran)
        //{
        //    if (SqlString == "")
        //    {
        //        return new DataTable();
        //    }
        //    DataSet MyDataResult = new DataSet();
        //    using (SqlDataAdapter MySqlDataAdapter = new SqlDataAdapter())
        //    {
        //        using (SqlCommand SelectCmd = new SqlCommand(SqlString, Conn, SqlConTran))
        //        {
        //            SelectCmd.CommandType = CommandType.Text;
        //            MySqlDataAdapter.SelectCommand = SelectCmd;
        //            MySqlDataAdapter.Fill(MyDataResult);
        //            return MyDataResult.Tables[0];
        //        }
        //    }
        //}
        //public object SqlExecuteInTrans(string SqlString, SqlConnection Conn, SqlTransaction Trans)
        //{
        //    object result;
        //    using (SqlCommand SqlCmd = new SqlCommand(SqlString, Conn, Trans))
        //    {
        //        SqlCmd.CommandType = CommandType.Text;
        //        result = SqlCmd.ExecuteScalar();
        //        return result;
        //    }
        //}



    }
}