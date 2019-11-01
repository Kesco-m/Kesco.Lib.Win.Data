using System.Data;

namespace Kesco.Lib.Win.Data.DALC.Directory
{
	public class BusinessProjectDALC : DALC
	{
		public BusinessProjectDALC(string connectionString) : base(connectionString)
		{
			tableName = "Справочники.dbo.БизнесПроекты";
			idField = "КодБизнесПроекта";
			nameField = "БизнесПроект";
		}

		public DataSet GetOpenBusinessProjects()
		{
			return GetData("SELECT " + idField + ", " + nameField + ", Parent, L, R FROM " + tableName +
					" WHERE (Закрыт = 0) ORDER BY L ASC", null);
		}
	}
}
