using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Directory
{
	public class AreaDALC : DALC
	{
		public AreaDALC(string connectionString) : base(connectionString)
		{
			idField = "КодТерритории";
			tableName = "Справочники.dbo.vwСтраны";
		}

		public DataTable GetAreas()
		{
			return GetDataTable("SELECT * FROM " + tableName, null);
		}
	}
}
