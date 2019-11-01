using System.Data;
using System.Data.SqlClient;
using Kesco.Lib.Win.Data.DALC.Directory;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
	/// <summary>
	/// ��������� ����, �������������� ��� �������� ����������
	/// </summary>
	public class PersonsUsedDALC : PersonDALC
	{
		private string personTableName;
		public PersonsUsedDALC(string connectionString) : base(connectionString)
		{
			personTableName = tableName;
			tableName = "���������.dbo.vw����������������";
		}

		#region Get Data

		public DataTable GetPersonsExtended()
		{
			return GetDataTable("DECLARE @Tbl1 TABLE (" + idField + " int)" +
			   " INSERT @Tbl1 SELECT " + idField + " FROM " + tableName + System.Environment.NewLine +

			   " DECLARE @Tbl2 TABLE (" + idField + " int, " + FromFieldName + " datetime, " + ToFieldName + " datetime)" + System.Environment.NewLine +

			   " INSERT @Tbl2 SELECT X." + idField + ", X." + FromFieldName + ", X." + ToFieldName +
			   " FROM @Tbl1 Y INNER JOIN " + OrgCardTable + " X ON Y." + idField + " = X." + idField + System.Environment.NewLine +

			   " INSERT @Tbl2 SELECT X." + idField + ", X." + FromFieldName + ", X." + ToFieldName + " " +
			   " FROM @Tbl1 Y INNER JOIN " + PersCardTable + " X ON Y." + idField + " = X." + idField + System.Environment.NewLine +

			   " SELECT * FROM @Tbl2", null);
		}

		public DataTable GetPersons()
		{
            //return GetDataTable("DECLARE @Tbl TABLE (" + idField + " int, " + nameField + " nvarchar(50), " + NameRLField + " varchar(400)) " + System.Environment.NewLine +
            //    " INSERT @Tbl SELECT " + idField + ", ' ', ' ' " + " FROM " + tableName + System.Environment.NewLine +
            //    " UPDATE @Tbl SET " + nameField + " = X." + nameField + ", " + NameRLField + " = X." + NameRLField +
            //    " FROM @Tbl Y INNER JOIN " + personTableName + " X ON X." + idField + " = Y." + idField + System.Environment.NewLine +
            //    " SELECT * FROM @Tbl ORDER BY " + nameField, null);

            return GetDataTable("IF OBJECT_ID(N'tempdb.dbo.#����������������') IS NOT NULL " + System.Environment.NewLine +
                               "    DELETE FROM #���������������� " + System.Environment.NewLine +
                               "ELSE " + System.Environment.NewLine +
                               "    CREATE TABLE #����������������(" + idField + " int, " + nameField + " nvarchar(50), " + NameRLField + " varchar(400)) " + System.Environment.NewLine +
                               " INSERT #���������������� SELECT " + idField + ", ' ', ' ' " + " FROM " + tableName + System.Environment.NewLine +
                               " UPDATE #���������������� SET " + nameField + " = X." + nameField + ", " + NameRLField + " = X." + NameRLField +
                               " FROM #���������������� Y INNER JOIN " + personTableName + " X (nolock) ON X." + idField + " = Y." + idField + System.Environment.NewLine +
                               " SELECT * FROM #���������������� ORDER BY " + nameField, null);
		}

		#endregion

		#region Change Data

		public bool UsePerson(int id)
		{
			return Exec("INSERT INTO " + tableName + " (" + idField + ") VALUES (@ID)",
			delegate(SqlCommand cmd)
			{
				AddParam(cmd, "@ID", SqlDbType.Int, id);
			});
		}

		#endregion
	}
}