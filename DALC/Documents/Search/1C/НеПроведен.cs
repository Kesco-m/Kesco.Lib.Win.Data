using System;
using System.Data.SqlClient;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search._1C
{
    [Option("1C.����������", typeof (����������))]
    public class ���������� : ValueOption
    {
        protected ����������(XmlElement el) : base(el)
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
NOT EXISTS (SELECT TOP 1 ������������
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
            int keyID = 0;
            int ind = 0;
            string ret = "#" + key;
            if (!int.TryParse(key, out keyID)) throw new Exception("Int is suspected.");
            using (var cn = new SqlConnection(Env.Buh.ConnectionString))
            using (
                var cmd = new SqlCommand("SELECT ����������� FROM �����������.dbo.vw�����������1� WHERE ������� =@ID",
                                         cn))
            {
                cmd.Parameters.AddWithValue("@ID", keyID);
                {
                    try
                    {
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read() && ind < 2)
                            {
                                ret = dr[0].ToString();
                                ind++;
                            }
                            dr.Close();
                            dr.Dispose();
                        }
                    }
                    catch (SqlException sqlEx)
                    {
                        Env.WriteSqlToLog(sqlEx, cmd);
                    }
                    catch (Exception ex)
                    {
                        Env.WriteToLog(ex);
                    }
                    finally
                    {
                        cn.Close();
                    }
                }
            }
            return ind == 1 ? ret : "#" + key;
        }
    }
}
