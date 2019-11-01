using System.Data;

namespace Kesco.Lib.Win.Data.DALC.Directory
{
	public class BusinessProjectDALC : DALC
	{
		public BusinessProjectDALC(string connectionString) : base(connectionString)
		{
			tableName = "�����������.dbo.�������������";
			idField = "����������������";
			nameField = "������������";
		}

		public DataSet GetOpenBusinessProjects()
		{
			return GetData("SELECT " + idField + ", " + nameField + ", Parent, L, R FROM " + tableName +
					" WHERE (������ = 0) ORDER BY L ASC", null);
		}
	}
}
