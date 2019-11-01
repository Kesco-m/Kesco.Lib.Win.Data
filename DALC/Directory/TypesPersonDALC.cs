using System.Data;

namespace Kesco.Lib.Win.Data.DALC.Directory
{
	public class TypesPersonDALC : DALC
	{
		public TypesPersonDALC(string connectionString) : base(connectionString)
		{
			tableName = "Справочники.dbo.vwТемыЛиц";
			idField = "КодТемыЛица";
			nameField = "ТемаЛица";
		}

		public DataSet GetAllTypes()
		{
			return GetData("SELECT " + idField + ", " + nameField + ", Parent, L, R FROM " + tableName + " (nolock)", null);
		}
	}
}
