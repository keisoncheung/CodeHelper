using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
using System.Web;

namespace PublicHelper
{ 
    public class SQLHelper
    {  
        
        //���ݿ������ַ���
        //static public readonly string conn = ConfigurationManager.ConnectionStrings["SQLConnectionString"].ConnectionString;
        // ���ڻ��������HASH��
        static private Hashtable parmCache = Hashtable.Synchronized(new Hashtable());
        /// <summary>
        ///  �������ӵ����ݿ��ü������ִ��һ��sql������������ݼ���
        /// </summary>
        /// <param name="connectionString">һ����Ч�������ַ���</param>
        /// <param name="commandText">�洢�������ƻ���sql�������</param>
        /// <param name="commandParameters">ִ���������ò����ļ���</param>
        /// <returns>ִ��������Ӱ�������</returns>
        static public int ExecuteNonQuery(string connectionString, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }
        /// <summary>
        /// �����е����ݿ�����ִ��һ��sql������������ݼ���
        /// </summary>
        /// <remarks>
        ///����:  
        ///  int result = ExecuteNonQuery(connString, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">һ�����е����ݿ�����</param>
        /// <param name="commandText">�洢�������ƻ���sql�������</param>
        /// <param name="commandParameters">ִ���������ò����ļ���</param>
        /// <returns>ִ��������Ӱ�������</returns>
        static public int ExecuteNonQuery(SqlConnection connection, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }
        /// <summary>
        ///ʹ�����е�SQL����ִ��һ��sql������������ݼ���
        /// </summary>
        /// <remarks>
        ///����:  
        ///  int result = ExecuteNonQuery(trans, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">һ�����е�����</param>
        /// <param name="commandText">�洢�������ƻ���sql�������</param>
        /// <param name="commandParameters">ִ���������ò����ļ���</param>
        /// <returns>ִ��������Ӱ�������</returns>
        static public int ExecuteNonQuery(SqlTransaction trans, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }
        /// <summary>
        /// ��ִ�е����ݿ�����ִ��һ���������ݼ���sql����
        /// </summary>
        /// <remarks>
        /// ����:  
        ///  OleDbDataReader r = ExecuteReader(connString, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">һ����Ч�������ַ���</param>
        /// <param name="commandText">�洢�������ƻ���sql�������</param>
        /// <param name="commandParameters">ִ���������ò����ļ���</param>
        /// <returns>��������Ķ�ȡ��</returns>
        static public SqlDataReader ExecuteReader(string connectionString, string cmdText, params SqlParameter[] commandParameters)
        {
            //����һ��SqlCommand����
            SqlCommand cmd = new SqlCommand();
            //����һ��SqlConnection����
            SqlConnection conn = new SqlConnection(connectionString);
            //������������һ��try/catch�ṹִ��sql�ı�����/�洢���̣���Ϊ��������������һ���쳣����Ҫ�ر����ӣ���Ϊû�ж�ȡ�����ڣ�
            //���commandBehaviour.CloseConnection �Ͳ���ִ��
            try
            {
                //���� PrepareCommand �������� SqlCommand �������ò���
                PrepareCommand(cmd, conn, null, cmdText, commandParameters);
                //���� SqlCommand  �� ExecuteReader ����
                SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                //�������
                cmd.Parameters.Clear();
                return reader;
            }
            catch (Exception e)
            {
                //�ر����ӣ��׳��쳣
                conn.Close();
                throw e;
            }
        }
        /// <summary>
        /// ����һ��DataSet���ݼ�
        /// </summary>
        /// <param name="connectionString">һ����Ч�������ַ���</param>
        /// <param name="cmdText">�洢�������ƻ���sql�������</param>
        /// <param name="commandParameters">ִ���������ò����ļ���</param>
        /// <returns>������������ݼ�</returns>
        static public DataSet ExecuteDataSet(string connectionString, string cmdText, params SqlParameter[] commandParameters)
        {
            //����һ��SqlCommand���󣬲�������г�ʼ��
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdText, commandParameters);
                //����SqlDataAdapter�����Լ�DataSet
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                try
                {
                    //���ds
                    da.Fill(ds);
                    // ���cmd�Ĳ������� 
                    cmd.Parameters.Clear();
                    //����ds
                    return ds;
                }
                catch
                {
                    //�ر����ӣ��׳��쳣
                    conn.Close();
                    throw;
                }
            }
        }
        /// <summary>
        /// ��ָ�������ݿ������ַ���ִ��һ���������һ�����ݼ��ĵ�һ��
        /// </summary>
        /// <remarks>
        ///����:  
        ///  Object obj = ExecuteScalar(connString, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        ///<param name="connectionString">һ����Ч�������ַ���</param>
        /// <param name="commandText">�洢�������ƻ���sql�������</param>
        /// <param name="commandParameters">ִ���������ò����ļ���</param>
        /// <returns>�� Convert.To{Type}������ת��Ϊ��Ҫ�� </returns>
        static public object ExecuteScalar(string connectionString, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }
        /// <summary>
        /// ��ָ�������ݿ�����ִ��һ���������һ�����ݼ��ĵ�һ��
        /// </summary>
        /// <remarks>
        /// ����:  
        ///  Object obj = ExecuteScalar(connString, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">һ�����ڵ����ݿ�����</param>
        /// <param name="commandText">�洢�������ƻ���sql�������</param>
        /// <param name="commandParameters">ִ���������ò����ļ���</param>
        /// <returns>�� Convert.To{Type}������ת��Ϊ��Ҫ�� </returns>
        static public object ExecuteScalar(SqlConnection connection, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, null, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }
        /// <summary>
        /// ������������ӵ�����
        /// </summary>
        /// <param name="cacheKey">��ӵ�����ı���</param>
        /// <param name="cmdParms">һ����Ҫ��ӵ������sql��������</param>
        static public void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }
        /// <summary>
        /// �һػ����������
        /// </summary>
        /// <param name="cacheKey">�����һز����Ĺؼ���</param>
        /// <returns>����Ĳ�������</returns>
        static public SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];
            if (cachedParms == null)
                return null;
            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];
            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms = (SqlParameter[])((ICloneable)cachedParms).Clone();
            return clonedParms;
        }
        /// <summary>
        /// ׼��ִ��һ������
        /// </summary>
        /// <param name="cmd">sql����</param>
        /// <param name="conn">Sql����</param>
        /// <param name="trans">Sql����</param>
        /// <param name="cmdText">�����ı�,���磺Select * from Products</param>
        /// <param name="cmdParms">ִ������Ĳ���</param>
        static private void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            //�ж����ӵ�״̬������ǹر�״̬�����
            if (conn.State != ConnectionState.Open)
                conn.Open();
            //cmd���Ը�ֵ
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            //�Ƿ���Ҫ�õ�������
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;
            //���cmd��Ҫ�Ĵ洢���̲���
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
    }
}
