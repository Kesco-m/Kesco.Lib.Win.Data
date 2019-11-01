using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Directory
{
	public class OPFormsDALC : DALC
	{
		public OPFormsDALC(string connectionString) : base(connectionString)
		{
			tableName = "�����������.dbo.������������";
			idField = "���������������";
			nameField = "������������";
		}

		public DataTable OPForms()
		{
			return GetDataTable("SELECT " + idField + ", " + nameField + " FROM " + tableName + " ORDER BY " + nameField + " ASC", null);
		}

		public DataRow OPForm(int id)
		{
			return GetRow(id);
		}
	}
}
