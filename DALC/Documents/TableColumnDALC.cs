using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
	/// <summary>
	/// DAL-компонент для доступа к view Документы.dbo.vwПоляТаблицыДокументыДанные
	/// </summary>
	public class TableColumnDALC : DALC
	{
		private const string typeField = "type";
		private const string colorderField = "colorder";

		public TableColumnDALC(string connectionString) : base(connectionString)
		{
			tableName = "Документы.dbo.vwПоляТаблицыДокументыДанные";

			idField = "";
			nameField = "name";
		}

		#region Accessors

		public string TypeField
		{
			get { return typeField; }
		}

		public string ColorderField
		{
			get { return colorderField; }
		}

		#endregion

		#region Get Data

		public DataSet GetFreeTableColumns(int docTypeID, string curColumn)
		{
			var fieldData = new FieldDALC(connectionString);

			return GetData("SELECT " + nameField + ", " + typeField +
				" FROM " + tableName + " WHERE (" +
				colorderField + " > " +
				"( SELECT " +
				colorderField +
				" FROM " + tableName +
				" WHERE " + nameField + " = '" + editedField + "'" +
				"))" +
				" AND " +
				"((" +
				nameField + " NOT IN " +
				"( SELECT " +
				fieldData.TableColumnField +
				" FROM " + fieldData.TableName +
				" WHERE " + fieldData.DocTypeIDField + " = @DocTypeID" +
				")) OR (" +
				nameField + " = @CurColumn))",
			   delegate(SqlCommand cmd)
			   {
				   AddParam(cmd, "@DocTypeID", SqlDbType.Int, docTypeID);
				   AddParam(cmd, "@CurColumn", SqlDbType.NVarChar, curColumn);
			   });
		}

		#endregion
	}
}