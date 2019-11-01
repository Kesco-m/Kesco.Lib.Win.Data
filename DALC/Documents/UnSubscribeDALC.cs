using System;
using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
	/// <summary>
	/// Summary description for SubscribeDALC.
	/// </summary>
	public class UnSubscribeDALC : DALC
	{
		public UnSubscribeDALC(string connectionString) : base(connectionString)
		{
			tableName = "dbo.»змен€ющие‘ормы";
			idField = " од»змен€ющей‘ормы";
		}

		public bool CheckIt(Guid GUID)
		{
			return FieldExists(" WHERE " + idField + " = @GUID",
							   delegate(SqlCommand cmd)
							   {
								   AddParam(cmd, "@GUID", SqlDbType.UniqueIdentifier, GUID);
							   });
		}
	}
}
