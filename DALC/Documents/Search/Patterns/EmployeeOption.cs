using System;
using System.Data.SqlClient;
using System.Resources;
using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns
{
    public class EmployeeOption : ValueOption
    {

        public EmployeeOption(XmlElement el) : base(el)
        {
        }

        public override string GetItemText(string key)
        {
            string s = null;
            var resources = new ResourceManager(typeof (EmployeeOption));
            using (var cm = new SqlCommand("SELECT " + resources.GetString("FIO") + " FROM Инвентаризация..Сотрудники WHERE КодСотрудника=@ID") {Connection = new SqlConnection(Settings.DS_document)})
            {
                cm.Parameters.AddWithValue("@ID", key);
                try
                {
                    cm.Connection.Open();
                    s = (string) cm.ExecuteScalar();
                    if (s.Length == 0) s = "#" + key;
                }
                catch (SqlException sqlEx)
                {
                    Env.WriteSqlToLog(sqlEx, cm);
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex);
                }
                finally
                {
                    cm.Connection.Close();
                    cm.Dispose();
                }
            }
            return s;
        }
    }
}
