using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Buhgalteriya
{
	public class Field1CDALC : DALC
	{
		internal string tablePartField = "��������������1�";
		internal string typeField = "���";
		internal string type1CIDField = "�������1����������";

		public Field1CDALC( string connectionString) : base( connectionString)
		{
			tableName = "vw����1�";
			idField = "�������1�";
			nameField = "����1�";
		}

		#region Accssesors

		public string TablePartField
		{
			get { return tablePartField;}
		}


		public string Type1CIDField
		{
			get { return type1CIDField;}
		}

		public string TypeField
		{
			get { return typeField;}
		}

		#endregion

		#region Get Data

		public DataSet GetEconomicField(int ecomonicID, int type1CID)
		{
		    using (var cmd = new SqlDataAdapter("SELECT " + idField + ", " + tablePartField + ", " + nameField + ", " + typeField + " FROM " + tableName + " WHERE (" + type1CIDField + " = @TypeID) AND (NOT EXISTS " + " (SELECT * FROM ��������������� " + " WHERE �������������� = @EconomicID AND " + idField + " = " + tableName + "." + idField + "))" + " ORDER BY " + tablePartField + ", " + typeField, new SqlConnection(connectionString)))
		    {
		        AddParam( cmd.SelectCommand, "@TypeID", SqlDbType.Int, type1CID);
		        AddParam( cmd.SelectCommand, "@EconomicID", SqlDbType.Int, ecomonicID);

		        return CMD_FillDS(cmd);
		    }
		}
		#endregion
	}
}
