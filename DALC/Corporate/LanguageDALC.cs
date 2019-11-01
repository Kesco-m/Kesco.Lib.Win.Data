using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Corporate
{
	public class LanguageDALC : DALC
	{
		private readonly string employeeTable;
		private readonly string employeeSIDField;
		private readonly string employeeLanguageField;

		public LanguageDALC(string connectionString) : base(connectionString)
		{
			tableName = "»нвентаризаци€.dbo.языки";
			idField = "язык";
			nameField = "ќписание";
			var data = new EmployeeDALC(connectionString);
			employeeTable = data.TableName;
			employeeSIDField = data.SIDField;
			employeeLanguageField = data.LanguageField;
		}

		#region Get Data

		public DataTable GetLanguages()
		{
			return GetDataTable( "SELECT " + idField + ", " + nameField +
				" FROM " + tableName + " ORDER BY " + nameField, null);
		}

		public string GetEmployeeLanguage()
		{
			return GetRecord<string>("SELECT " + employeeLanguageField +
				" FROM " + employeeTable + " (NOLOCK) WHERE " + employeeSIDField + " = SUSER_SID()",
				null, null);
		}

		#endregion

		#region Change Data

		public bool ChangeLanguage(string language)
		{
			var cmd = new SqlCommand("UPDATE " + employeeTable +
				" SET " + employeeLanguageField + " = @Lang WHERE " + employeeSIDField + " =  SUSER_SID()",
				new SqlConnection(connectionString));
			AddParam(cmd, "@Lang", SqlDbType.Char, language);

			return CMD_Exec(cmd);
		}

		#endregion
	}
}