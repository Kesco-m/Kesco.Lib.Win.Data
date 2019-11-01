using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
	public struct SettingsPrintForm
	{
		int typeID;
		int printID;
		public int TypeID
		{
			get { return typeID; }
		}

		public int PrintID
		{
			get { return printID; }
		}
		public SettingsPrintForm(int typeID, int printID)
		{
			this.typeID = typeID;
			this.printID = printID;
		}
	}
	/// <summary>
	/// Summary description for SettingsPrintFormDALC.
	/// </summary>
	public class SettingsPrintFormDALC : DALC
	{
		public SettingsPrintFormDALC(string connectionString) : base(connectionString)
		{
			tableName = "ƒокументы.dbo.vwЌастройкиѕечатных‘орм";

			idField = " од“ипаƒокумента";
			nameField = " одѕечатной‘ормы";
		}

		#region Get Data

		public SettingsPrintForm[] GetSettings(int docTypeID)
		{
			return GetRecords<SettingsPrintForm>("SELECT " + idField + ", " + nameField + " FROM " + tableName + " (nolock) WHERE " + idField + " = @typeID", 
				delegate(SqlCommand cmd)
			{
				AddParam(cmd, "@typeID", SqlDbType.Int, docTypeID);
			}, 
			delegate(IDataRecord dr)
			{
				return new SettingsPrintForm((int)dr[0], (int)dr[1]);
			}).ToArray();
		}

		#endregion

		#region Change Data

		public  bool DeleteSettings(SettingsPrintForm[] settings)
		{
			string query = settings.Aggregate("", (current, t) => current + (" DELETE FROM " + tableName + " WHERE " + idField + " = " + t.TypeID.ToString() + " AND " + nameField + " = " + t.PrintID.ToString() + " "));
		    return Exec(query);
		}

		public bool SetSettings(SettingsPrintForm[] settings)
		{
			string query = settings.Aggregate("", (current, t) => current + (" IF NOT EXISTS(SELECT * FROM " + tableName + " WHERE " + idField + " = " + t.TypeID.ToString() + " AND " + nameField + " = " + t.PrintID.ToString() + ") " + " INSERT INTO " + tableName + " (" + idField + ", " + nameField + ") VALUES (" + t.TypeID.ToString() + ", " + t.PrintID.ToString() + ") "));

		    return Exec(query);
		}

		#endregion
	}
}
