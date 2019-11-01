using System;
using System.Data.SqlClient;
using System.Resources;
using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns
{
    public class EmployeeTwoValueListOption : TwoListValueOption
    {
        public EmployeeTwoValueListOption(XmlElement el) : base(el)
        {
        }

        public override string GetItemText(string key)
        {
            string s;
            var resources = new ResourceManager(typeof (EmployeeListOption));
            var cm =
                new SqlCommand("SELECT " + resources.GetString("FIO") +
                               " FROM Инвентаризация..Сотрудники WITH(NOLOCK) WHERE КодСотрудника=" + key)
                    {Connection = new SqlConnection(Settings.DS_document)};
            try
            {
                cm.Connection.Open();
                s = (string) cm.ExecuteScalar();
                if (s.Length == 0) s = "#" + key;
            }
            catch (SqlException sex)
            {
                Env.WriteSqlToLog(sex, cm);
                s = "#" + key;
            }
            catch (Exception ex)
            {
                Env.WriteToLog(ex);
                s = "#" + key;
            }
            finally
            {
                cm.Connection.Close();
                cm.Dispose();
            }
            return s;
        }
    }
}
