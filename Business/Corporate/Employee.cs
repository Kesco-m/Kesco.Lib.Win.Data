using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
//using Kesco.Lib.Win.Data.Types;

namespace Kesco.Lib.Win.Data.Business.Corporate
{
	/// <summary>
	/// ���������
	/// </summary>
	public class Employee : Base
	{
		string sex;
		string fio;
		string iof;
		string email;

		#region ACCSESSORS

		/// <summary>
		/// ������� �.�.
		/// </summary>
		public string Name
		{
			get
			{
				LoadIfDelayed();
				return (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.Equals("ru")?fio:iof);
			}
		}

		/// <summary>
		/// ����� ����������� �����
		/// </summary>
		public string EMail
		{
			get
			{
				LoadIfDelayed();
				return email;
			}
		}

		#endregion

		public DataTable Find(string text)
		{
			var s = new Types.KString(text);
			var dt = new DataTable();
			string query = "SELECT TOP 13 " + ID_Field + "," + fioField + "," + iofField + " FROM " + FillFrom_Table + " WHERE ' '+�������+' '+���+' '+�������� LIKE '" + s.SqlLikeWords + "'";
			using(SqlCommand cmd = new SqlCommand(query))
			using(cmd.Connection = new SqlConnection(Settings.DS_user))
			{
				try
				{
					cmd.Connection.Open();
					using(SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
					{
						if(dr.Read())
						{
							dt.Load(dr);
							dr.Close();
						}
					}
				}
				catch(SqlException sex)
				{
					Env.WriteSqlToLog(sex, cmd);
				}
				catch(Exception ex)
				{
					Env.WriteToLog(ex);
				}
			}
			return dt;
		}

		public Employee(int id) : base(id) { }

		#region DB
		protected override string FillFrom_Table
		{
			get
			{
				return "��������������.dbo.����������";
			}
		}

		protected override string ID_Field
		{
			get
			{
				return "�������������";
			}
		}

	    private const string fioField = "���";
	    private const string iofField = "IOF";
	    private const string emailField = "Email";
	    private const string sexField = "���";

	    protected override void Fill(DataRow row)
		{
			base.Fill(row);
			fio = (string)row[fioField];
			iof = (string)row[iofField];
			email = row.IsNull(emailField) ? "" : (string)row[emailField];
			sex = row.IsNull(sexField) ? "�" : (string)row[sexField];
		}

		#endregion
	}
}
