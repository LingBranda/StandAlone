using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Standalone
{
    internal class Functionality
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["CorpitConnectionString"].ConnectionString;
        private int mint_CommandTimeout = 30;
        private SqlConnection mobj_SqlConnection;
        private SqlCommand mobj_SqlCommand;
        private string mstr_ConnectionString;

        public void CloseConnection()
        {
            if (mobj_SqlConnection.State != ConnectionState.Closed) mobj_SqlConnection.Close();
        }

        public int ExecuteNonQuery(string str, SqlParameter[] prams)
        {
            int ret;
            mstr_ConnectionString = ConnectionString;
            using (SqlConnection connection = new SqlConnection(mstr_ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(str, connection);

                cmd.Parameters.AddRange(prams);
                try
                {

                    if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    else if (connection.State == System.Data.ConnectionState.Broken)
                    {
                        connection.Close();
                        connection.Open();
                    }

                    ret = cmd.ExecuteNonQuery();
                }
                catch (SqlException E)
                {
                    throw new Exception(E.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
            return ret;
        }

        public DataSet GetDatasetByCommand(string sCommand, string sDatatable)
        {
            try
            {
                mstr_ConnectionString = ConnectionString;

                mobj_SqlConnection = new SqlConnection(mstr_ConnectionString);
                mobj_SqlCommand = new SqlCommand();
                mobj_SqlCommand.CommandTimeout = mint_CommandTimeout;
                mobj_SqlCommand.Connection = mobj_SqlConnection;

                mobj_SqlCommand.CommandText = sCommand;
                mobj_SqlCommand.CommandTimeout = mint_CommandTimeout;

                mobj_SqlConnection.Open();

                SqlDataAdapter adpt = new SqlDataAdapter(mobj_SqlCommand);
                DataSet ds = new DataSet();
                adpt.Fill(ds, sDatatable);
                return ds;
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}