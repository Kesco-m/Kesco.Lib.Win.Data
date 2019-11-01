using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Kesco.Lib.Win.Data.DALC.Corporate;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    public class FolderDALC : TreeDALC
    {
        protected const string shareTable = "Документы.dbo.ПапкиДокументовОбщийДоступ";
        protected const string shareIDField = "КодПапкиДокументовОбщегоДоступа";
        protected const string sp_unreadTable = "sp_ПапкиДокументовНепрочитано";
        protected const string clientIDField = "КодСотрудника";
        protected const string rightsField = "Права";
        protected const string allDocsCountField = "Документов";
        protected const string unreadField = "Непрочитано";
        protected string empIDField;

        private EmployeeDALC employeeData;

        protected string selectString;
        protected string orderString;

        public FolderDALC(string connectionString)
            : base(connectionString)
        {
            tableName = "Документы.dbo.vwПапкиДокументов";

            idField = "КодПапкиДокументов";
            nameField = "ПапкаДокументов";

            employeeData = new EmployeeDALC(null);
            empIDField = employeeData.IDField;

            selectString =
                "SELECT " +
                idField + ", " +
                nameField + ", " +
                "ISNULL(" + parentField + ", 0) AS " + parentField + ", " +
                editorField + ", " +
                editedField +
                " FROM " + tableName + " WITH(NOLOCK)";

            orderString = " ORDER BY L";
        }

        #region Accessors

        public string ShareTable
        {
            get { return shareTable; }
        }

        public string ShareIDField
        {
            get { return shareIDField; }
        }

        public string ClientIDField
        {
            get { return clientIDField; }
        }

        public string RightsField
        {
            get { return rightsField; }
        }

        public string AllDocsCountField
        {
            get { return allDocsCountField; }
        }

        public string UnreadField
        {
            get { return unreadField; }
        }

        #endregion

        #region Get Data

        public bool FolderExists(int folderID)
        {
            return FieldExists(folderID);
        }

		public DataSet GetFolders(int empID, CancellationToken ct)
		{
			return GetTreeData(selectString + " WHERE " + empIDField + " = @EmpID" + orderString,
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@EmpID", SqlDbType.Int, empID);
				}, ct);
		}

        public string GetFolderName(int folderID)
        {
            return GetField(
                "SELECT " + nameField + " FROM " + tableName + " WITH(NOLOCK)" + " WHERE " + idField + " = @FolderID",
                nameField,
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@FolderID", SqlDbType.Int, folderID);
                    }) as string;
        }

		public DataTable GetFolders()
		{
			using(var cn = new SqlConnection(connectionString))
			using(SqlCommand cmd = new SqlCommand(sp_unreadTable, cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = cmdTimeout })
			{
				var table = new DataTable(tableName);
				table.BeginLoadData();

				try
				{
					cmd.Connection.Open();
					using(SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
					{
						using(DataTable dtSchema = reader.GetSchemaTable())
						{
							if(dtSchema != null)
							{
								foreach(DataRow drow in dtSchema.Rows)
								{
									string columnName = Convert.ToString(drow["ColumnName"]);
									DataColumn column = new DataColumn(columnName, (Type)(drow["DataType"]))
															{
																Unique = (bool)drow["IsUnique"],
																AllowDBNull = (bool)drow["AllowDBNull"],
																AutoIncrement = (bool)drow["IsAutoIncrement"],
																ReadOnly = false
															};
									table.Columns.Add(column);
								}
								table.PrimaryKey = new DataColumn[] { table.Columns[clientIDField], table.Columns[idField] };
							}
						}
						table.Load(reader);
						reader.Close();
					}
					for(int i = 0; i < table.Columns.Count; i++)
						if(table.Columns[i].ReadOnly)
							table.Columns[i].ReadOnly = false;
				}
				catch(SqlException sqlEx)
				{
					if(sqlEx.Class == 11 && sqlEx.Number == 0)
						table.Clear();
					else
						ProcessSqlEx(sqlEx, cmd);
				}
				catch(Exception ex)
				{
					ErrorMessage(ex, null, "CMD_FillDT");
				}
				finally
				{
					if(cmd.Connection.State != ConnectionState.Closed)
						cmd.Connection.Close();
					table.EndLoadData();
				}
				return table;
			}
		}

        /// <summary>
        /// Метод загружает данные о рабочих папках, количествах документах, колчичества непрочитанных документах в папках.
        /// Вызывается из Environment
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
		public bool RefreshFolders(ref DataTable table)
		{
			using(var cn = new SqlConnection(connectionString))
			using(var cmd = new SqlCommand(sp_unreadTable, cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = cmdTimeout })
			{
				table.BeginLoadData();

				try
				{
#if AdvancedLogging
                    Log.Logger.EnterMethod(this, "FolderDALC RefreshFolders(ref DataTable table)");
#endif

					cmd.Connection.Open();

					using(SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
					{
						table.Load(reader, LoadOption.OverwriteChanges);
						reader.Close();
					}

					return true;
				}
				catch(SqlException sqlEx)
				{
					if(sqlEx.Class == 11 && sqlEx.Number == 0)
						table.Clear();
					else
						ProcessSqlEx(sqlEx, cmd);
				}
				catch(Exception ex)
				{
					ErrorMessage(ex, cmd, "CMD_FillDT");
				}
				finally
				{
					if(cmd.Connection.State != ConnectionState.Closed)
						cmd.Connection.Close();
					table.EndLoadData();

#if AdvancedLogging
                    Log.Logger.LeaveMethod(this, "FolderDALC RefreshFolders(ref DataTable table)");
#endif
				}

				return false;
			}
		}

        public DataSet GetChildFolders(int parentID)
        {
            string query = selectString + " WHERE " + parentField;

            if (parentID > 0)
                query += " = @ParentID";
            else
                query += " IS NULL";

            return GetData(query + orderString,
                           delegate(SqlCommand cmd)
                               {
                                   if (parentID > 0)
                                       AddParam(cmd, "@ParentID", SqlDbType.Int, parentID);
                               });
        }

        public DataSet GetParentFolders(int id)
        {
            return GetData(
                "SELECT " + idField +
                " FROM " + tableName +
                " WHERE " + leftField + " <= " +
                "(SELECT " + leftField + " FROM " + tableName + " WHERE " + idField + " = @ID)" +
                " AND " + rightField + " >= " +
                "(SELECT " + rightField + " FROM " + tableName + " WHERE " + idField + " = @ID)" +
                orderString,
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@ID", SqlDbType.Int, id);
                    });
        }

        public DataTable GetClients(int id)
        {
            return GetDataTable("SELECT " +
                                shareIDField + ", " +
                                clientIDField + ", " +
                                rightsField +
                                " FROM " + shareTable +
                                " WHERE " + idField + " = @FolderID",
                                delegate(SqlCommand cmd)
                                    {
                                        AddParam(cmd, "@FolderID", SqlDbType.Int, id);
                                    });
        }

        #endregion

        #region Change Data

        public int New(int id, string text)
        {
            string cmdText = id != 0
                                 ? "INSERT INTO " + tableName + " (" + nameField + ", " + parentField + ")" +
                                   " VALUES (@Name, @ParentID)"
                                 : "INSERT INTO " + tableName + " (" + nameField + ")" +
                                   " VALUES (@Name)";

			cmdText += " SELECT SCOPE_IDENTITY() AS " + identityField;

            return ExecID(cmdText,
                          delegate(SqlCommand cmd)
                              {
                                  AddParam(cmd, "@Name", SqlDbType.NVarChar, text);
                                  if (id != 0)
                                      AddParam(cmd, "@ParentID", SqlDbType.Int, id);
                              }, 0);
        }

        public bool UnsortedMove(int what, int where)
        {
            using (
                var cmd =
                    new SqlCommand(
                        "UPDATE " + tableName + " SET " + parentField + " = @ParentID" + " WHERE " + idField + " = @ID",
                        new SqlConnection(connectionString)))
            {
                AddParam(cmd, "@ParentID", SqlDbType.Int, @where);
                AddParam(cmd, "@ID", SqlDbType.Int, what);

                return CMD_Exec(cmd);
            }
        }

        public bool SortedMove(int what, int before)
        {
            using (
                var cmd =
                    new SqlCommand(
                        "UPDATE " + tableName + " SET " + parentField + " = (" + "SELECT " + parentField + " FROM " +
                        tableName + " WHERE " + idField + " = @BeforeID" + "), " + leftField + " = (" + "SELECT " +
                        leftField + " FROM " + tableName + " WHERE " + idField + " = @BeforeID)" + " WHERE " + idField +
                        " = @ID", new SqlConnection(connectionString)))
            {
                AddParam(cmd, "@BeforeID", SqlDbType.Int, before);
                AddParam(cmd, "@ID", SqlDbType.Int, what);

                return CMD_Exec(cmd);
            }
        }

        public int AddShare(int folderID, int clientID, int rights)
        {
            return
                GetIdentityField(
                    "INSERT INTO " + shareTable + " (" + idField + ", " + clientIDField + ", " + rightsField + ")" +
					" VALUES (@FolderID, @ClientID, @Rights) SELECT SCOPE_IDENTITY() AS " + identityField,
                    delegate(SqlCommand cmd)
                        {
                            AddParam(cmd, "@FolderID", SqlDbType.Int, folderID);
                            AddParam(cmd, "@ClientID", SqlDbType.Int, clientID);
                            AddParam(cmd, "@Rights", SqlDbType.TinyInt, Convert.ToByte(rights));
                        });
        }

        public bool SetRights(int id, int rights)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = new SqlConnection(connectionString);
                cmd.CommandText =
                    "UPDATE " + shareTable +
                    " SET " + rightsField + " = @Rights" +
                    " WHERE " + shareIDField + " = @ID";

                AddParam(cmd, "@ID", SqlDbType.Int, id);
                AddParam(cmd, "@Rights", SqlDbType.TinyInt, Convert.ToByte(rights));

                return CMD_Exec(cmd);
            }
        }

        public bool RemoveShare(int id)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = new SqlConnection(connectionString);
                cmd.CommandText =
                    "DELETE " + shareTable +
                    " WHERE " + shareIDField + " = @ID";

                AddParam(cmd, "@ID", SqlDbType.Int, id);

                return CMD_Exec(cmd);
            }
        }

        #endregion
    }
}