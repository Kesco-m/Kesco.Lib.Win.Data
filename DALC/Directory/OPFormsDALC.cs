using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Directory
{
	public class OPFormsDALC : DALC
	{
		public OPFormsDALC(string connectionString) : base(connectionString)
		{
			tableName = "—правочники.dbo.ќргѕрав‘ормы";
			idField = " одќргѕрав‘ормы";
			nameField = "ќргѕрав‘орма";
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
