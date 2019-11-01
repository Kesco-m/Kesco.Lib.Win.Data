using System.Data;

namespace Kesco.Lib.Win.Data.DALC.Corporate
{
	/// <summary>
	/// DAL-��������� ��� ������� � ������� ��������������.dbo.����.
	/// </summary>
	public class RuleDALC : DALC
	{
	    private const string appointField = "���������";

	    public RuleDALC(string connectionString): base(connectionString)
		{
			tableName = "��������������.dbo.vw����";

			idField = "�������";
			nameField = "����";
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
			return GetData("SELECT 0 " + idField + ", '���' " + nameField + " UNION " +
					"SELECT " + idField + ", " + nameField + " FROM " + tableName, null);
		}

		#endregion
	}
}
