using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Buhgalteriya
{
	public class Type1CDALC : DALC
	{
		internal string typeField = "���";

		public Type1CDALC( string connectionString) : base( connectionString)
		{
		    tableName = "dbo.����1�";
		    idField = "�������1�";
		    nameField = "���";
		}

	    #region Accessories

		public string TypeField
		{
			get { return typeField;}
		}

		#endregion

		#region Get Data

		public DataSet GetDocType1C()
		{
			var cmd = new SqlDataAdapter("SELECT * " +
				"FROM " + tableName +
				" WHERE " + typeField + " = '��������'",
				new SqlConnection( connectionString));
			return CMD_FillDS(cmd);
		}

		#endregion
	}
}
