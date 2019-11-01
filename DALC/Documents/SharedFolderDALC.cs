using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
	/// <summary>
    /// DAL-компонент для доступа к таблице Документы.dbo.vwПапкиДокументовОбщийДоступ
	/// </summary>
	public class SharedFolderDALC : FolderDALC
	{
        private const string ownerIDField = "КодСотрудника"; // кто расшарил
        private const string originalTable = "Документы.dbo.ПапкиДокументовОбщийДоступ";

		public SharedFolderDALC(string connectionString)
			: base(connectionString)
		{
			tableName = "Документы.dbo.vwПапкиДокументовОбщийДоступ";

			selectString =
				"SELECT " +
					idField + ", " +
					nameField + ", " +
					"ISNULL(" + parentField + ", 0) AS " + parentField + ", " +
					ownerIDField + ", " +
					rightsField + ", " +
					editorField + ", " +
					editedField +
				" FROM " + tableName;
		}

		#region Get Data

		public DataSet GetFullAccessFolders(int empID)
		{
			return GetTreeData(selectString +
				" WHERE " + rightsField + " = 1" +
				" AND " + empIDField + " = @EmpID" +
				orderString,
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
				});
		}

		public int GetOwnerID(int swfID)
		{
			return GetIntField("SELECT " + ownerIDField +
				" FROM " + tableName +
				" WHERE " + idField + " = @ID",
				ownerIDField,
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@ID", SqlDbType.Int, swfID);
				});
		}

		public bool Rights(int swfID, int empID)
		{
			return GetRecord<byte>(
				"SELECT " + rightsField +
				" FROM " + originalTable +
				" WHERE " + idField + " = @ID AND " + ownerIDField + " = @EmpID",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@ID", SqlDbType.Int, swfID);
					AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
				}, null) > 0;
		}

		#endregion
	}
}