using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image
{
	/// <summary>
	/// Summary description for Хранилище.
	/// </summary>
	[Option("Image.Хранилище", typeof(Хранилище))]
	public class Хранилище : ValueOption
	{

		protected Хранилище(XmlElement el)
			: base(el)
		{
			emptyValueText = Resources.GetString("emptyValueText");

			UseSubNodes = UseSubNodes;
			shortTextPrefix = Resources.GetString("shortTextPrefix");
			shortTextPostfix = "";

			htmlPrefix = Resources.GetString("htmlPrefix");
			htmlPostfix = "";

			textItemPrefix = "[";
			textItemPostfix = "]";
		}

		private bool UseSubNodes
		{
			get { return el.GetAttribute("subnodes").ToLower().Equals("true"); }
			set { el.SetAttribute("subnodes", value.ToString().ToLower()); }
		}

		public override string GetItemText(string key)
		{
			return Regex.IsMatch(key, "^\\d+$")
					   ? GetName(int.Parse(key)) + (UseSubNodes ? Resources.GetString("GetItemText") : "")
					   : key;
		}

		public override bool OpenWindow()
		{
			return false;
		}

		public override string GetSQL(bool throwOnError)
		{
			return null;
		}

		public string GetSQLCondition2(string pattern)
		{
			Regex r;
			string ids = "";
			string val = Value;

			r = new Regex("^\\d{1,9}$", RegexOptions.IgnoreCase);
			int id = (r.IsMatch(val) ? int.Parse(val) : 0);

			bool all = id == 0;

			if(!all)
				if(UseSubNodes)
				{
					const string sql = @"
                        DECLARE @L int, @R int
                        SELECT @L = L, @R = R FROM Документы.dbo.Хранилища WHERE КодХранилища = @id
                        SELECT КодХранилища FROM Документы.dbo.Хранилища WHERE @L<=L AND R<=@R
                        ";
					using(var cmd = new SqlCommand(sql))
					using(cmd.Connection = new SqlConnection(Settings.DS_document))
					{
						cmd.Parameters.AddWithValue("@id", id);
						try
						{
							cmd.Connection.Open();
							using(SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
							{
								while(dr.Read())
									ids += (ids.Length == 0 ? "" : ",") + dr[0];
								dr.Close();
								dr.Dispose();
							}
						}
						catch(SqlException sex)
						{
							Env.WriteSqlToLog(sex, cmd);
							return null;
						}
						catch(Exception ex)
						{
							Env.WriteToLog(ex);
							return null;
						}
						finally
						{
							if(cmd.Connection.State == ConnectionState.Open)
								cmd.Connection.Close();
						}
					}
				}
				else
				{
					ids = id.ToString();
				}

			return all ? @"" + pattern + " > 0" : @"" + pattern + @" IN (" + ids + @")";
		}

		public override string GetShortText()
		{
			if(Value.Length == 0)
				return shortTextPrefix + textItemPrefix + emptyValueText + textItemPostfix + shortTextPostfix;
			return base.GetShortText();
		}

		public static string GetName(int id)
		{
			string ret;
			using(SqlCommand cm = new SqlCommand("SELECT Хранилище FROM Документы.dbo.Хранилища WHERE КодХранилища = @id"))
			using(cm.Connection = new SqlConnection(Settings.DS_document))
			{
				cm.Parameters.AddWithValue("@id", id);
				try
				{
					cm.Connection.Open();
					ret = (string)cm.ExecuteScalar();
					if(ret.Length == 0)
						ret = "#" + id;
				}
				catch(SqlException sex)
				{
					ret = "#" + id;
					Env.WriteSqlToLog(sex, cm);
				}
				catch(Exception ex)
				{
					ret = "#" + id;
					Env.WriteToLog(ex);
				}
				finally
				{
					if(cm.Connection.State != ConnectionState.Closed)
						cm.Connection.Close();
				}
			}
			return ret;
		}
	}
}