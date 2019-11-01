using System.Data;

namespace Kesco.Lib.Win.Data.DALC.Corporate
{
	/// <summary>
	/// DAL-компонент для доступа к таблице Инвентаризация.dbo.Роли.
	/// </summary>
	public class RuleDALC : DALC
	{
	    private const string appointField = "Назначать";

	    public RuleDALC(string connectionString): base(connectionString)
		{
			tableName = "Инвентаризация.dbo.vwРоли";

			idField = "КодРоли";
			nameField = "Роль";
		}

		#region Accessors

		public string AppointField
		{
			get { return appointField;}
		}

		#endregion

		#region Get Data

		public DataSet GetRules()
		{
			return GetData("SELECT 0 " + idField + ", 'Нет' " + nameField + " UNION " +
					"SELECT " + idField + ", " + nameField + " FROM " + tableName, null);
		}

		#endregion
	}
}
