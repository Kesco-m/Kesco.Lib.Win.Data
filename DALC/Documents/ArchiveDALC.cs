using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
	/// <summary>
    /// DAL-компонент для доступа к таблице Документы.dbo.Хранилища
	/// </summary>
	public class ArchiveDALC : TreeDALC
	{
        private const string fnFullName = "dbo.fn_Tree_Хранилища_FullPath";

		public ArchiveDALC(string connectionString) : base(connectionString)
		{
			tableName = "Документы.dbo.Хранилища";

			idField = "КодХранилища";
			nameField = "Хранилище";
		}

		#region Get Data

		public string GetArchive(int id)
		{
		    return GetRecord<string>("SELECT " + fnFullName + "(@ID, 0) " + nameField,
		                             delegate(SqlCommand cmd) { AddParam(cmd, "@ID", SqlDbType.Int, id); },
		                             delegate(IDataRecord dr)
		                                 { return dr.IsDBNull(0) ? "#" + id.ToString() : dr[0].ToString(); });
		}

	    #endregion
	}
}