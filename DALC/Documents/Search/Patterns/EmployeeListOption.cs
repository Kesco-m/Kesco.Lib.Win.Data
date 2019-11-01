using System;
using System.Data.SqlClient;
using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns
{
    public class EmployeeListOption : ListOption
    {

        public EmployeeListOption(XmlElement el)  : base(el)
        {
        }

        public override string GetItemText(string key)
        {
            string s = null;
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(EmployeeListOption));
			using(SqlCommand cm = new SqlCommand( "SELECT " + resources.GetString("FIO") + " FROM Инвентаризация..Сотрудники WHERE КодСотрудника=" + key))
			using(cm.Connection =  new SqlConnection(Settings.DS_document))
            {
                try
                {
                    cm.Connection.Open();
                    s = (string) cm.ExecuteScalar();
                    if (s.Length == 0) s = "#" + key;
                }
                catch (SqlException sex)
                {
                    Env.WriteSqlToLog(sex, cm);
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex);
                    s = "#" + key;
                }
                finally
                {
                    cm.Connection.Close();
                }
            }
            return s;
        }
    }
}