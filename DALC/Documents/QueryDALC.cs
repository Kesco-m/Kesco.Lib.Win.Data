using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Kesco.Lib.Win.Data.DALC.Corporate;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    public class QueryDALC : DALC
    {
        private string empIDField;
        private const string xmlField = "XML";

        private EmployeeDALC employeeData;

        public QueryDALC(string connectionString) : base(connectionString)
        {
            tableName = "Документы.dbo.vwЗапросы";

            idField = "КодЗапроса";
            nameField = "Запрос";

            employeeData = new EmployeeDALC(null);
            empIDField = employeeData.IDField;
        }

        #region Accessors

        public string EmpIDField
        {
            get { return empIDField; }
        }

        public string XMLField
        {
            get { return xmlField; }
        }

        #endregion

        #region Get Data

		public DataTable GetSearchParamSets(int employeeID)
		{
			return GetDataTable("SELECT " + idField + ", " + nameField + ", " + empIDField + ", " + xmlField +
				" FROM " + tableName + " WHERE " + empIDField + " = @EmpID" + " ORDER BY " + nameField,
				 cmd =>
				{
					AddParam(cmd, "@EmpID", SqlDbType.Int, employeeID);
				});
		}

        public int GetIDByName(string name, int employeeID)
        {
            return GetIntField("SELECT " +   idField + " FROM " + tableName + " WHERE " + nameField + " = @Name" +
                " AND " + empIDField + " = @EmpID",
                idField,
                cmd =>
                    {
                        AddParam(cmd, "@Name", SqlDbType.NVarChar, name);
                        AddParam(cmd, "@EmpID", SqlDbType.Int, employeeID);
                    });
        }

		public List<string> GetSearchParameteries()
		{
			return GetRecords<string>("SELECT НаборПараметровПоиска FROM vwНаборыПараметровПоиска", null,
				dr => { return dr.GetString(0); });
		}

        #endregion

        #region Change Data

        public bool Save(string name, string xml, int empID, ref int id)
        {
            var cmd = new SqlCommand();
            cmd.Connection = new SqlConnection(connectionString);

            if (id > 0)
                cmd.CommandText = @"
UPDATE " + tableName + @"	
SET
	" + nameField + @"=@name,
	" + empIDField +
                                  @"=@uid,
	[" + xmlField + @"]=@xml
WHERE " + idField + @"=@id";
            else
                cmd.CommandText = @"
INSERT " + tableName + "(" + nameField + "," + empIDField + ",[" + xmlField +
                                  @"]) VALUES(@name,@uid,@xml)
SET @id=@@IDENTITY";

            AddParam(cmd, "@name", SqlDbType.NVarChar, name);
            AddParam(cmd, "@uid", SqlDbType.Int, empID);
            AddParam(cmd, "@xml", SqlDbType.NVarChar, xml);
            AddParam(cmd, "@id", SqlDbType.Int, id);
            cmd.Parameters["@id"].Direction = ParameterDirection.InputOutput;

            if (!CMD_Exec(cmd)) return false;
            id = (int) cmd.Parameters["@id"].Value;
            return true;
        }

        #endregion
    }
}