using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Document
{
	[Option("ТипДокумента", typeof(ТипДокумента))]
	public class ТипДокумента : ListOption
	{
		protected ТипДокумента(XmlElement el)
			: base(el)
		{
			el.SetAttribute("value", NormalizeS(el.GetAttribute("value")));
			//удалить повторяющиеся
			mask = "^[1-9][0-9]{0,9}[c]{0,1}$"; //чиселка
			shortTextPrefix = "";
			shortTextPostfix = "";

			htmlPrefix = Resources.GetString("htmlPrefix");
			htmlPrefix2 = Resources.GetString("htmlPrefix2");
			htmlPostfix = "";

			textItemPrefix = "[";
			textItemPostfix = "]";

		}

		public override string GetItemText(string key)
		{
			Match m;
			if (!(m = Regex.Match(key, "^(\\d{1,9})([C]{0,1})$", RegexOptions.IgnoreCase)).Success)
				return "";

			return GetText(m.Groups[1].Value) +
				((m.Groups[2].Value.Length > 0) ? Resources.GetString("GetItemText") : "");
		}

		public override string GetSQL(bool throwOnError)
		{
			string val = NormalizeC(Value);

			if (val.Length == 0)
			{
				if (throwOnError) throw new Exception(Resources.GetString("GetSQL"));
				else return null;
			}


			return "T0.КодТипаДокумента IN (" + val + ")";
		}




		public static string NormalizeC(string src)
		{
			Match m;
			int idValue;
			var ids = new List<int>();
			var b = new StringBuilder();
			foreach (string id in src.Split(','))
			{
				if (!(m = Regex.Match(id, "^(\\d{1,9})([C]{0,1})$", RegexOptions.IgnoreCase)).Success) continue;
				idValue = 0;
				if (int.TryParse(m.Groups[1].Value, out idValue) && idValue > 0)
				{
					if (ids.Contains(idValue))
						continue;
				}
				else
					continue;
				if (m.Groups[2].Value.Length == 0)
				{
					ids.Add(idValue);
					if (b.Length > 0) b.Append(",");
					b.Append(m.Groups[1].Value);
				}
				else
				{
					foreach (DataRow r in GetChildren(m.Groups[1].Value))
					{
						if (r[0] is int && (idValue = (int)r[0]) > 0)
						{
							if (!ids.Contains(idValue))
								ids.Add(idValue);
							else
								continue;
							if (b.Length > 0) b.Append(",");
							b.Append(r[0].ToString());
						}
						else
							continue;
					}
				}
			}
			return b.ToString();
		}
		public static string NormalizeS(string src)
		{
			Match m;
			int idValue;
			var ids = new List<int>();
			var b = new StringBuilder();
			foreach (string id in src.Split(','))
			{
				if (!(m = Regex.Match(id, "^(\\d{1,9})([S]{0,1})([C]{0,1})$", RegexOptions.IgnoreCase)).Success) continue;
				idValue = 0;
				if (int.TryParse(m.Groups[1].Value, out idValue) && idValue > 0)
				{
					if (ids.Contains(idValue))
						continue;
				}
				else
					continue;
				if (m.Groups[2].Value.Length == 0)
				{
					ids.Add(idValue);
					if (b.Length > 0) b.Append(",");
					b.Append(m.Groups[1].Value + m.Groups[3].Value);
				}
				else
				{
					foreach (DataRow r in GetSimiliar(m.Groups[1].Value))
					{
						if (r[0] is int && (idValue = (int)r[0]) > 0)
						{
							if (!ids.Contains(idValue))
								ids.Add(idValue);
							else
								continue;

							if (b.Length > 0) b.Append(",");
							b.Append(r[0] + m.Groups[3].Value);
						}
						else
							continue;
					}
				}
			}
			return b.ToString();
		}


		public static string GetText(string id)
		{
			int ind = 0;
			string ret = null;
			var resources = new ResourceManager(typeof(ТипДокумента));
			if (!Regex.IsMatch(id, "^\\d{0,9}$")) throw new Exception("Int is suspected.");
			var cmd = new SqlCommand("SELECT " + resources.GetString("GetText") + " FROM Документы.dbo.ТипыДокументов WHERE КодТипаДокумента = @ID", new SqlConnection(Settings.DS_document));
			cmd.Parameters.AddWithValue("@ID", id);
			try
			{
				cmd.Connection.Open();
				SqlDataReader dr = cmd.ExecuteReader();
				if (dr != null)
				{
					if (ind < 2 && dr.Read())
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
				ind = 0;
			}
			catch (Exception ex)
			{
				Env.WriteToLog(ex);
				ind = 0;
			}
			finally
			{
				cmd.Connection.Close();
				cmd.Dispose();
			}
			if (ind == 1) return ret;
			return "#" + id;
		}

		static DataRow[] GetSimiliar(string id)
		{
		    int idValue = 0;
		    if (!int.TryParse(id, out idValue)) throw new Exception("Int is suspected.");
		    if (idValue <= 0) return null;
		    var dt = new DataTable();
		    const string sql =
		        @"
                DECLARE  @Parent int
                SET @Parent = (SELECT CASE WHEN ТипДокумента IS NULL THEN КодТипаДокумента ELSE Parent END AS Код
                FROM Документы.dbo.ТипыДокументов WHERE КодТипаДокумента=@ID)

                SELECT КодТипаДокумента 
                FROM Документы.dbo.ТипыДокументов
                WHERE (@Parent IS NULL AND КодТипаДокумента=@ID) OR (@Parent IS NOT NULL AND Parent = @Parent)
                ";
		    using (var da = new SqlDataAdapter(sql, Settings.DS_document))
		    {
		        try
		        {
		            da.SelectCommand.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int) {Value = idValue});
		            da.Fill(dt);
		        }
		        catch (SqlException sqlEx)
		        {
		            Env.WriteSqlToLog(sqlEx, da.SelectCommand);
		        }
		        catch (Exception ex)
		        {
		            Env.WriteToLog(ex);
		        }
		        finally
		        {
		            da.SelectCommand.Connection.Close();
		            da.Dispose();
		        }
		    }

		    return dt.Select();
		}

	    static DataRow[] GetChildren(string id)
	    {
	        int idValue = 0;
	        if (!int.TryParse(id, out idValue)) throw new Exception("Int is suspected.");
	        if (idValue <= 0) return null;
	        var dt = new DataTable();
	        const string sql =
	            @"
                SELECT T0.КодТипаДокумента 
                FROM Документы.dbo.ТипыДокументов AS T0
                INNER JOIN  Документы.dbo.ТипыДокументов AS T1 ON T1.L<=T0.L AND T0.R<=T1.R
                WHERE T1.КодТипаДокумента=@ID";

	        using (var da = new SqlDataAdapter(sql, Settings.DS_document))
	        {
	            try
	            {
	                da.SelectCommand.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int) {Value = idValue});
	                da.Fill(dt);
	            }
	            catch (SqlException sqlEx)
	            {
	                Env.WriteSqlToLog(sqlEx, da.SelectCommand);
	            }
	            catch (Exception ex)
	            {
	                Env.WriteToLog(ex);
	            }
	            finally
	            {
	                da.SelectCommand.Connection.Close();
	                da.Dispose();
	            }
	        }

	        return dt.Select();
	    }
	}
}
