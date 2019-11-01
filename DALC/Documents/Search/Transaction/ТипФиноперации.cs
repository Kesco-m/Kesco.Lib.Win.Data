using System;
using System.Data.SqlClient;
using System.Threading;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Transaction
{
	/// <summary>
	/// Summary description for ТипФиноперации.
	/// </summary>
	[Option("Transaction.ТипФиноперации", typeof(ТипФиноперации))]
	public class ТипФиноперации:ValueOption
	{
		protected ТипФиноперации(XmlElement el):base(el)
		{
			emptyValueText=Resources.GetString("emptyValueText");

			shortTextPrefix= Resources.GetString("shortTextPrefix");
			shortTextPostfix="";

			htmlPrefix=Resources.GetString("htmlPrefix");
			htmlPostfix="";

			textItemPrefix="[";
			textItemPostfix="]";
		}

		public  override string GetSQL(bool throwOnError)
		{

		    return
		        @"EXISTS (SELECT *
                FROM dbo.vwТранзакции T1 WITH(NOLOCK)
                WHERE T1.КодДокументаОснования = T0.КодДокумента" +
		                        ((Value.Length > 0) ? "\n AND (КодТипаТранзакции = " + Value + "))" : ")") +
		        @" OR EXISTS (SELECT *
                FROM dbo.vwТранзакции T1 WITH(NOLOCK)
                WHERE T1.КодДокументаПодтверждения = T0.КодДокумента " +
		                        ((Value.Length > 0) ? "\n AND (КодТипаТранзакции = " + Value + "))" : ")");
		}

	    public override string GetItemText(string key)
		{
			int id = 0;
			if (!int.TryParse(key, out id) || id < 1) throw new Exception("Int is suspected.");

			using (var cn = new SqlConnection(Settings.DS_document))
			using (var cmd = new SqlCommand("SELECT " + (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.Equals("ru") ? "ТипТранзакции" : "ТипТранзакцииEN") + " FROM Документы.dbo.ТипыТранзакций WHERE КодТипаТранзакции = " + key, cn))
			{
				try
				{
					cmd.Connection.Open();
					object str = cmd.ExecuteScalar();
					if (str != DBNull.Value && !string.IsNullOrEmpty(str.ToString()))
						return str.ToString();
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
					cmd.Connection.Close();
				}
			}
			return "#" + key;
		}
	}
}
