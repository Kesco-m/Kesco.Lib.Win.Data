using System;
using System.Data;
using System.Data.SqlClient;
using Kesco.Lib.Log;

namespace Kesco.Lib.Win.Data.Business.V2
{
    public delegate void FillByIDDelegate(DataTable dt, int id);

    public delegate void FindDelegate(Dso filter, int[] ids, ref int startRecord, ref int maxRecords, DataTable dt);

    public class DBModule : MarshalByRefObject
    {
        private string connectionString;

        public string ConnectionString
        {
            get { return connectionString; }
        }

        public DBModule()
        {
            // онструктор дл€ удаленной активации
        }

        public void Find(string query, DataTable dt)
        {
            using (var da = new SqlDataAdapter(query, connectionString))
            {
                try
                {
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    throw new DetailedException(ex.Message + " ConnectionString:" + connectionString, ex,
                                                da.SelectCommand);
                }
            }
        }

        public IDataReader GetReader(string query)
        {
            using (var cm = new SqlCommand(query))
            {
                cm.Connection = new SqlConnection(ConnectionString);
                cm.Connection.Open();
                return cm.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public ReaderHelper GetReaderHelper(string query)
        {
            return new ReaderHelper(GetReader(query));
        }

        public void Find(Dso filter, DataTable dt)
        {
            using (var da = new SqlDataAdapter())
            {
                da.SelectCommand = filter.GetSqlCommand();
                da.SelectCommand.Connection = new SqlConnection(connectionString);
                try
                {
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    throw new DetailedException(ex.Message, ex, da.SelectCommand);
                }
            }
        }

        public void Find(Dso filter, int[] ids, ref int startRecord, ref int maxRecords, DataTable dt)
        {
            Find(filter.GetSqlCommand(), ids, ref startRecord, ref maxRecords, dt);
        }

        protected void Find(SqlCommand cm, int[] ids, ref int startRecord, ref int maxRecords, DataTable dt)
        {
            cm.Connection = new SqlConnection(ConnectionString);

            SqlDataReader r = null;
            object[] values = null;
            int i = 0;
            int n = 0;
            try
            {
                cm.Connection.Open();
                r = cm.ExecuteReader();

                if (dt != null)
                {
                    dt.BeginLoadData();
                    if (dt.Columns.Count == 0)
                        foreach (DataRow row in r.GetSchemaTable().Rows)
                        {
                            dt.Columns.Add((string) row["ColumnName"], (Type) row["DataType"]);
                        }

                    values = new object[r.FieldCount];
                }
                while (r.Read())
                {
                    if (startRecord <= i && n < maxRecords)
                    {
                        ids[n] = (int) r[0];

                        if (dt != null)
                        {
                            r.GetValues(values);
                            dt.LoadDataRow(values, true);
                        }
                        n++;
                    }
                    i++;
                }
                if (dt != null) dt.EndLoadData();
            }
            catch (Exception ex)
            {
                throw new DetailedException(ex.Message, ex, cm);
            }
            finally
            {
                if (r != null) r.Close();
                cm.Connection.Close();
            }

            startRecord = i;
            maxRecords = n;
        }

        public void ExecuteReader(string cmdText, IReaderListener listener)
        {
            using (var cm = new SqlCommand(cmdText))
            {
                cm.Connection = new SqlConnection(ConnectionString);
                try
                {
                    cm.Connection.Open();
                    using (SqlDataReader r = cm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (r.Read()) listener.OnRead(r);
                        r.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw new DetailedException(ex.Message, ex, cm);
                }
                finally
                {
                    cm.Connection.Close();
                }
            }
        }

        public string ExecuteScalar(string cmdText)
        {
            using (var cm = new SqlCommand(cmdText))
            using (cm.Connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    cm.Connection.Open();
                    object obj = cm.ExecuteScalar();
                    if (obj == null) return null;
                    else if (obj == DBNull.Value) return "";
                    else return Entity.Object2Str(obj);
                }
                catch (Exception ex)
                {
                    throw new DetailedException(ex.Message, ex, cm);
                }
                finally
                {
                    cm.Connection.Close();
                }
            }
        }

        public DBModule(string connectionString)
        {
            this.connectionString = connectionString;
        }
    }
}
