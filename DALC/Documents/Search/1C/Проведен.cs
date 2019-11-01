using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search._1C
{
    [Option("1C.��������", typeof (��������))]
    public class �������� : ValueOption
    {
        protected ��������(XmlElement el) : base(el)
        {
            emptyValueText = Resources.GetString("emptyValueText");

            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPostfix = "";

            textItemPrefix = "[";
            textItemPostfix = "]";
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"
EXISTS (SELECT TOP 1 ������������
	FROM ��������������������� T1 WHERE T1.������������ = T0.������������ 
		AND ������� = 0" +
                ((Value.Length > 0) ? "\n		AND (�������������� = " + Value + ")\n" : "\n") +
                @"UNION
	SELECT TOP 1 ������������
	FROM ��������������������� T1 WHERE T1.������������ = T0.������������
		AND ������� = 0" +
                ((Value.Length > 0) ? "\n		AND (�������������� = " + Value + "))" : ")");
        }

        public override string GetItemText(string key)
        {
            int ind = 0;
            string ret = null;
            if (!Regex.IsMatch(key, "^\\d{0,9}$")) throw new Exception("Int is suspected.");
            Console.WriteLine("{0}: {1}", DateTime.Now.ToString("HH:mm:ss fff"), Settings.DS_Buh);
            var cmd = new SqlCommand("SELECT ����������� FROM �����������.dbo.vw�����������1� WHERE ������� = @ID",
                                     new SqlConnection(Env.Buh.ConnectionString));
            cmd.Parameters.AddWithValue("@ID", key);
            try
            {
                cmd.Connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read() && ind < 2)
                {
                    ind++;
                    ret = dr[0].ToString();
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number != 50000)
                    Env.WriteSqlToLog(sqlEx, cmd);
            }
            catch (Exception ex)
            {
                Env.WriteToLog(ex);
            }
            return ind == 1 ? ret : "#" + key;
        }
    }
}
