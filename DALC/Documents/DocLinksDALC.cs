using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Kesco.Lib.Win.Data.DALC.Corporate;
using Kesco.Lib.Win.Data.Documents;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    /// <summary>
    /// DAL-компонент для доступа к таблице Документы.dbo.СвязиДокументов
    /// </summary>
    public class DocLinksDALC : DALC
    {
        private const string parentDocIDField = "КодДокументаОснования";
        private const string childDocIDField = "КодДокументаВытекающего";
        private const string subFieldIDField = "КодПоляДокумента";
        private const string parentOrderField = "ПорядокОснования";
        private const string childOrderField = "ПорядокВытекающего";
        private const string subFieldNameField = "ТипДокументаПоля";
        private const string subFieldNameEngField = "TypeDocПоля";

        private string subFieldField;
        private string subFieldOrderField;
        private string empIDField;
        private string empField;
        private string empTable;
        private string inWorkField;

        private const string tableNameUpdDel = "Документы.dbo.vwСвязиДокументовПорядок";

        private string fieldTable;
        private string docTable;
        private string typesTable;
        private string documentIDField;
        private string documentNameField;
        private string documentDateField;
        private string documentDescriptionField;
        private string typesIDField;
        private string typesNameField;
        private string typesNameEngField;
        private string spDocRights;
        private string spSendMessage;

        private const string spMakeDocsLink = "sp_MakeDocsLink";
        private const string spCheckDocsLink = "sp_CheckDocsLink";

        public DocLinksDALC(string connectionString)
            : base(connectionString)
        {
            tableName = "vwСвязиДокументов";

            idField = "КодСвязиДокументов";
            nameField = "";
            FieldDALC fieldData;
            EmployeeDALC empData;
            MessageDALC messData;
            DocTypeDALC typesData;
            fieldData = new FieldDALC(connectionString);
            fieldTable = fieldData.TableName;

            subFieldField = fieldData.NameField;
            subFieldOrderField = fieldData.PositionField;

            var docData = new DocumentDALC(connectionString);
            docTable = docData.TableName;
            documentIDField = docData.IDField;
            documentNameField = docData.NameField;
            documentDateField = docData.DateField;
            documentDescriptionField = docData.DescriptionField;
            spDocRights = docData.SpDocRights;

            inWorkField = docData.InWorkField;

            messData = new MessageDALC(connectionString);
            spSendMessage = messData.SpSendMessage;

            empData = new EmployeeDALC(connectionString);
            empIDField = empData.IDField;
            empField = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.Equals("ru") ? empData.NameField : empData.EmloyeeField;
            empTable = empData.TableName;

            typesData = new DocTypeDALC(connectionString);
            typesTable = typesData.TableName;
            typesIDField = typesData.IDField;
            typesNameField = typesData.NameField;
            typesNameEngField = typesData.TypeDocField;
        }

        #region Accessors

        public string ParentDocIDField
        {
            get { return parentDocIDField; }
        }

        public string ChildDocIDField
        {
            get { return childDocIDField; }
        }

        public string SubFieldIDField
        {
            get { return subFieldIDField; }
        }

        public string SubFieldNameField
        {
            get { return subFieldNameField; }
        }

        public string SubFieldNameEngField
        {
            get { return subFieldNameEngField; }
        }

        public string SubFieldField
        {
            get { return subFieldField; }
        }

        public string SubFieldOrderField
        {
            get { return subFieldOrderField; }
        }

        public string ParentOrderField
        {
            get { return parentOrderField; }
        }

        public string ChildOrderField
        {
            get { return childOrderField; }
        }

        public string EmpIDField
        {
            get { return empIDField; }
        }

        public string EmpField
        {
            get { return empField; }
        }

        public string DocumentIDField
        {
            get { return documentIDField; }
        }

        #endregion

        #region Get Data

        public List<int> GetLinksData(int docID)
        {
            return GetRecords<int>("SELECT TOP 1 " + parentDocIDField + ", 0 FROM " + tableName +
                                   " WHERE " + tableName + "." + childDocIDField + " = @DocID " +
                                   "UNION SELECT TOP 1 0, " + childDocIDField + " FROM " + tableName +
                                   " WHERE " + tableName + "." + parentDocIDField + " = @DocID ",
                                   delegate(SqlCommand cmd)
                                       {
                                           AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                                       },
                                   dr => dr[0].Equals(0) ? -1 : 1);
        }

		public DataSet GetMainLinksData(int docID)
		{
			bool ru = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.Equals("ru");
			return GetData("DECLARE @L Table(" + idField + " int, " + documentIDField + " int, " + subFieldIDField +
				 " int, Порядок int, " + editorField + " int, " + editedField + " datetime) " +
				 "INSERT @L  " +
				 "SELECT " + idField + ", " + parentDocIDField + ", " + subFieldIDField + ", " + parentOrderField + ", " +
				 editorField + ", " + editedField +
				 " FROM " + tableName + " (NOLOCK) WHERE " + childDocIDField + " = @DocID " +

				 "SELECT " + subFieldIDField + ", ПорядокПоляДокумента, " + (ru ? "'поле '" : "'field '") +
				 "+ПолеДокумента" + (ru ? "" : "EN") + " Поле  " +
				 "FROM ПоляДокументов " +
				 "WHERE " + subFieldIDField + " IN (SELECT DISTINCT " + subFieldIDField + " FROM @L WHERE " +
				 subFieldIDField + " IS NOT NULL) " +

				 "SELECT L.КодСвязиДокументов, L." + subFieldIDField + ", L.КодДокумента, vwДокументы." + typesIDField +
				 ", НазваниеДокумента, " + (ru ? typesNameField : typesNameEngField) + " " + typesNameField +
				 ", НомерДокумента, ДатаДокумента, Описание, 0 Direction," + empTable + "." + empField + ", L." +
				 editedField +
				 "  FROM vwДокументы (NOLOCK) " +
				 "       INNER JOIN ТипыДокументов (NOLOCK) ON vwДокументы." + typesIDField + " = ТипыДокументов." +
				 typesIDField + "  " +
				 "       INNER JOIN @L L ON vwДокументы.КодДокумента = L.КодДокумента " +
				 " LEFT JOIN " + empTable + " ON L." + editorField + "=" + empTable + "." + empIDField +
				 " WHERE L." + subFieldIDField + " IS NULL " +
				 "ORDER BY L.Порядок",
				 delegate(SqlCommand cmd)
				 {
					 AddParam(cmd, "@DocID", SqlDbType.Int, docID);
				 });
		}

		public DataSet GetMainLinksData(int docID, int subFieldID)
		{
			return GetData("SELECT " + tableName + ".КодСвязиДокументов, vwДокументы.КодДокумента, " + tableName + "." +
						subFieldIDField +
						", НазваниеДокумента, " +
						(Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.Equals("ru")
							 ? typesNameField
							 : typesNameEngField) + " " + typesNameField +
						", НомерДокумента, ДатаДокумента, Описание, 0 Direction, " + empTable + "." + empField + ", " +
						tableName + "." + editedField +
						" FROM vwДокументы (NOLOCK)" +
						"       INNER JOIN ТипыДокументов (NOLOCK) ON vwДокументы." + typesIDField +
						" = ТипыДокументов." + typesIDField + "  " +
						"       INNER JOIN " + tableName + " (NOLOCK) ON vwДокументы.КодДокумента = " + tableName +
						".КодДокументаОснования " +
						" LEFT JOIN " + empTable + " ON " + tableName + "." + editorField + "=" + empTable + "." +
						empIDField +
						" WHERE " + tableName + "." + subFieldIDField +
						" = @SubFieldID AND КодДокументаВытекающего = @DocID " +
						"ORDER BY " + tableName + ".ПорядокОснования",
				 delegate(SqlCommand cmd)
				 {
					 AddParam(cmd, "@DocID", SqlDbType.Int, docID);
					 AddParam(cmd, "@SubFieldID", SqlDbType.Int, subFieldID);
				 });

		}

		public DataSet GetChildLinksData(int docID)
		{
			bool ru = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.Equals("ru");
			return GetData("DECLARE @L Table(КодСвязиДокументов int, КодДокумента int, " + subFieldIDField +
				  " int, Порядок int, " + editorField + " int, " + editedField + " datetime) " +
				  "INSERT @L  " +
				  "SELECT КодСвязиДокументов, КодДокументаВытекающего, " + subFieldIDField + ", " + childOrderField +
				  ", " + editorField + ", " + editedField +
				  " FROM " + tableName + " (NOLOCK) WHERE КодДокументаОснования = @DocID  " +

				  "DECLARE @P Table(" + subFieldIDField + " int, ПорядокПоляДокумента int, Поле varchar(156))  " +
				  "INSERT @P  " +
				  "SELECT " + subFieldIDField + ", ПорядокПоляДокумента," +
				  (ru ? typesNameField + "+' поле '" : typesNameEngField + "+' field '") + "+ПолеДокумента" +
				  (ru ? "" : "EN") + " Поле  " +
				  "FROM ПоляДокументов (NOLOCK) INNER JOIN ТипыДокументов (NOLOCK) ON ПоляДокументов." + typesIDField +
				  " = ТипыДокументов." + typesIDField + "  " +
				  "WHERE " + subFieldIDField + " IN (SELECT DISTINCT " + subFieldIDField + " FROM @L WHERE " +
				  subFieldIDField + " IS NOT NULL) " +

				  "SELECT * FROM @P  " +
				  "ORDER BY ПорядокПоляДокумента \n" +

				  "SELECT L.КодСвязиДокументов, L." + subFieldIDField + ", L.КодДокумента, vwДокументы." +
				  typesIDField + ", НазваниеДокумента, " + (ru ? typesNameField : typesNameEngField) + " " +
				  typesNameField +
				  ", НомерДокумента, ДатаДокумента, Описание, 1 Direction," + empTable + "." + empField + ", L." +
				  editedField +
				  " FROM vwДокументы (NOLOCK) " +
				  "       INNER JOIN ТипыДокументов (NOLOCK) ON vwДокументы." + typesIDField + " = ТипыДокументов." +
				  typesIDField + "  " +
				  "       INNER JOIN @L L ON vwДокументы.КодДокумента = L.КодДокумента  " +
				  " LEFT JOIN " + empTable + " ON L." + editorField + "=" + empTable + "." + empIDField +
				  " WHERE L." + subFieldIDField + " IS NULL " +
				  "ORDER BY L.Порядок",
			   delegate(SqlCommand cmd)
			   {
				   AddParam(cmd, "@DocID", SqlDbType.Int, docID);
			   });
		}

		public DataSet GetChildLinksData(int docID, int subFieldID)
		{
			return GetData("SELECT " + tableName + ".КодСвязиДокументов, vwДокументы.КодДокумента, " + tableName + "." +
					subFieldIDField +
					", НазваниеДокумента, " +
					(Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.Equals("ru")
						 ? typesNameField
						 : typesNameEngField) + " " + typesNameField +
					", НомерДокумента, ДатаДокумента, Описание, " + tableName + ".ПорядокВытекающего, 1 Direction, " +
					empTable + "." + empField + ", " + tableName + "." + editedField +
					" FROM vwДокументы (NOLOCK) " +
					"       INNER JOIN ТипыДокументов (NOLOCK) ON vwДокументы." + typesIDField + " = ТипыДокументов." +
					typesIDField + "  " +
					"       INNER JOIN " + tableName + " (NOLOCK) ON vwДокументы.КодДокумента = " + tableName +
					".КодДокументаВытекающего " +
					" LEFT JOIN " + empTable + " ON " + tableName + "." + editorField + "=" + empTable + "." +
					empIDField +
					" WHERE " + tableName + "." + subFieldIDField + " = @SubFieldID AND КодДокументаОснования = @DocID " +
					"ORDER BY " + tableName + ".ПорядокВытекающего",
				 delegate(SqlCommand cmd)
				 {
					 AddParam(cmd, "@DocID", SqlDbType.Int, docID);
					 AddParam(cmd, "@SubFieldID", SqlDbType.Int, subFieldID);
				 });
		}

        public DataTable GetLinksDocs(int docID, bool root, bool main)
        {
            bool ru = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.Equals("ru");
            string sqlstrParent = "SELECT DISTINCT " +
                                  docTable + "." + documentIDField + ", " + docTable + "." + typesIDField + ", " +
                                  (ru ? typesNameField : typesNameEngField) + " " + typesNameField + ", " +
                                  "CASE WHEN " + documentNameField + " <> '' THEN " + documentNameField + " ELSE " +
                                  (ru ? typesNameField : typesNameEngField) + " END " + documentNameField + ", " +
                                  documentDateField + ", НомерДокумента, " +
                                  "REPLACE(" + docTable + "." + documentDescriptionField +
                                  ", CHAR(13) + CHAR(10), CHAR(160) + ' ') AS " + documentDescriptionField +
                                  ", КодИзображенияДокументаОсновного\n" +
                                  "FROM " + docTable + " (NOLOCK)\nINNER JOIN " + tableName +
                                  "(NOLOCK) ON " + docTable + "." + documentIDField + " = " + tableName + "." +
                                  documentIDField + "Основания\n" +
                                  "INNER JOIN " + typesTable + " (NOLOCK) ON " + docTable + "." + typesIDField + " = " +
                                  typesTable + "." + typesIDField + "\n" +
                                  "WHERE " + tableName + "." + childDocIDField + " = @DocID ";
            string sqlstrChild = "SELECT DISTINCT " +
                                 "" + docTable + "." + documentIDField + ", " + docTable + "." + typesIDField + ", " +
                                 (ru ? typesNameField : typesNameEngField) + " " + typesNameField + ", " +
                                 "CASE WHEN " + documentNameField + " <> '' THEN " + documentNameField + " ELSE " +
                                 (ru ? typesNameField : typesNameEngField) + " END " + documentNameField + ", " +
                                 documentDateField + ", НомерДокумента, " +
                                 "REPLACE(" + docTable + "." + documentDescriptionField +
                                 ", CHAR(13) + CHAR(10), CHAR(160) + ' ') AS " + documentDescriptionField +
                                 ", КодИзображенияДокументаОсновного\n" +
                                 "FROM " + docTable + " (NOLOCK)\nINNER JOIN " + tableName +
                                 " (NOLOCK) ON " + docTable + "." + documentIDField + " = " + tableName + "." +
                                 documentIDField + "Вытекающего\n" +
                                 "INNER JOIN " + typesTable + " ON " + docTable + "." + typesIDField + " = " +
                                 typesTable + "." + typesIDField + "\n" +
                                 "WHERE " + tableName + "." + parentDocIDField + " = @DocID";

            string sqlstr = root ? sqlstrParent + "UNION " + sqlstrChild : (main ? sqlstrParent : sqlstrChild);

            return GetDataTable(sqlstr, delegate(SqlCommand cmd)
                                            {
                                                AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                                            });
        }

        public bool HasLinks(int docID)
        {
            return FieldExists("WHERE " + parentDocIDField + " = @docID OR " + childDocIDField + " = @docID",
                               delegate(SqlCommand cmd)
                                   {
                                       AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                                   });
        }

        public bool HasContractLinks(int docID)
        {
            return GetIntField(
                "SELECT TOP 1 " + tableName + "." + parentDocIDField + " FROM " + tableName +
                " WITH (NOLOCK) INNER JOIN " +
                " " + docTable + " WITH (NOLOCK) ON " + tableName + "." + parentDocIDField + " = " + docTable + "." +
                documentIDField + " INNER JOIN " +
                " " + typesTable + " WITH (NOLOCK) ON " + docTable + "." + typesIDField + " = " + typesTable + "." +
                typesIDField + " INNER JOIN " +
                " " + typesTable + " Parent WITH (NOLOCK) ON " + typesTable + ".L >= Parent.L AND " + typesTable +
                ".R <= Parent.R " +
                " WHERE Parent." + typesIDField + " = 2039 AND " + tableName + "." + childDocIDField + " = @DocID",
                parentDocIDField,
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                    }) > 0;
        }

        public DataSet GetHeventRights(int docID, int linkDocID)
        {
            using (var conn = new SqlConnection(connectionString))
            using (
                var cmd =
                    new SqlDataAdapter(
                        "create table #t1 (" + empIDField + " int, " + empField + " varchar(50), " + inWorkField +
                        " int)" +
                        "create table #t2 (" + empIDField + " int, " + empField + " varchar(50), " + inWorkField +
                        " int)" +
                        "insert into #t1 exec " + spDocRights + " @DocID " + "insert into #t2 exec " + spDocRights +
                        " @LinkDocID " + "select * from #t1 where " + empIDField + " not in (select " + empIDField +
                        " from #t2)" + "select * from #t2 where " + empIDField + " not in (select " + empIDField +
                        " from #t1)", conn))
            {
                AddParam(cmd.SelectCommand, "@DocID", SqlDbType.Int, docID);
                AddParam(cmd.SelectCommand, "@LinkDocID", SqlDbType.Int, linkDocID);

                return CMD_FillDS(cmd);
            }
        }

        public DataSet GetLinkIDs(int docID)
        {
            return GetData(
                "SELECT DISTINCT " +
                parentDocIDField + " AS " + documentIDField +
                " FROM " + tableName + " (nolock) " +
                " WHERE " + childDocIDField + " = @DocID" +
                " UNION " +
                " SELECT DISTINCT " +
                childDocIDField + " AS " + documentIDField +
                " FROM " + tableName +
                " (NOLOCK) WHERE " + parentDocIDField + " = @DocID",
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                    });
        }

        public bool CheckDocLinkExists(int parentDocID, int childDocID)
        {
            return GetCount("SELECT TOP 1 " +
                            idField + " AS " + countField +
                            " FROM " + tableName +
                            " (NOLOCK) WHERE " + childDocIDField + " = @КодДокументаВытекающего AND " + parentDocIDField +
                            " = @КодДокументаОснования",
                            delegate(SqlCommand cmd)
                                {
                                    AddParam(cmd, "@КодДокументаОснования", SqlDbType.Int, parentDocID);
                                    AddParam(cmd, "@КодДокументаВытекающего", SqlDbType.Int, childDocID);
                                }) > 0;
        }

        public bool HasExistsDocLink(int parentDocID, int childDocID)
        {
            return GetCount("SELECT TOP 1 " +
                            idField + " AS " + countField +
                            " FROM " + tableName +
                            " (NOLOCK) WHERE (" + childDocIDField + " = @КодДокументаВытекающего AND " +
                            parentDocIDField + " = @КодДокументаОснования) OR (" + parentDocIDField +
                            " = @КодДокументаВытекающего AND " + childDocIDField + " = @КодДокументаОснования)",
                            delegate(SqlCommand cmd)
                                {
                                    AddParam(cmd, "@КодДокументаОснования", SqlDbType.Int, parentDocID);
                                    AddParam(cmd, "@КодДокументаВытекающего", SqlDbType.Int, childDocID);
                                }) > 0;
        }

		public object CheckDocLink(int parentDocID, int childDocID)
		{
			using(var conn = new SqlConnection(connectionString))
			using(var cmd = new SqlCommand(spCheckDocsLink, conn))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				AddParam(cmd, "@КодДокументаОснования", SqlDbType.Int, parentDocID);
				AddParam(cmd, "@КодДокументаВытекающего", SqlDbType.Int, childDocID);

				cmd.Parameters.Add(new SqlParameter("@ReturnValue", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue });

				try
				{
					cmd.Connection.Open();
					using(SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
					{
						try
						{
							object obj = cmd.Parameters["@ReturnValue"].Value;
							if(obj != null && obj is int)
							{
								var result = (int)obj;
								if(result == 3)
								{
									var dt = new DataTable("Документы");
									dt.Load(reader);
									return dt;
								}
								else
									return result;
							}
						}
						finally
						{
							reader.Close();
						}
					}
				}
				catch(SqlException sqlEx)
				{
					ProcessSqlEx(sqlEx, cmd, "CheckDocLink", false);
				}
				finally
				{
					if(conn.State != ConnectionState.Closed)
						cmd.Connection.Close();
				}
			}

			return null;
		}

        #endregion

        #region Change Data

        public bool AddDocLink(int parentDocID, int childDocID)
        {
            return Exec(spMakeDocsLink,
                        delegate(SqlCommand cmd)
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                AddParam(cmd, "@ParentDocID", SqlDbType.Int, parentDocID);
                                AddParam(cmd, "@ChildDocID", SqlDbType.Int, childDocID);
                                AddParam(cmd, "@FildID", SqlDbType.Int, DBNull.Value);
                            });
        }

        public bool ChangeParentDocLinkOrder(int firstLinkID, int secondLinkID)
        {
            using (var conn = new SqlConnection(connectionString))
            using (
                var cmd =
                    new SqlCommand(
                        "declare @s1 int, @s2 int" + " select @s1 = " + parentOrderField +
                        " from dbo.vwСвязиДокументовПорядок where КодСвязиДокументов = @FirstLinkID" + " select @s2 = " +
                        parentOrderField + " from dbo.vwСвязиДокументовПорядок where КодСвязиДокументов = @SecondLinkID" +
                        " UPDATE " + tableNameUpdDel + " SET " + parentOrderField + " = CASE" + " WHEN " + idField +
                        " = @FirstLinkID THEN @s2" + " WHEN " + idField + " = @SecondLinkID THEN @s1" + " END" +
                        " WHERE " +
                        idField + " IN ( @FirstLinkID, @SecondLinkID)", conn))
            {
                AddParam(cmd, "@FirstLinkID", SqlDbType.Int, firstLinkID);
                AddParam(cmd, "@SecondLinkID", SqlDbType.Int, secondLinkID);

                return CMD_Exec(cmd);
            }
        }

        public bool ChangeChildDocLinkOrder(int firstLinkID, int secondLinkID)
        {
            return Exec("declare @s1 int, @s2 int" +
                        " select @s1 = " + childOrderField +
                        " from dbo.vwСвязиДокументовПорядок where КодСвязиДокументов = @FirstLinkID" +
                        " select @s2 = " + childOrderField +
                        " from dbo.vwСвязиДокументовПорядок where КодСвязиДокументов = @SecondLinkID" +
                        " UPDATE " + tableNameUpdDel + " SET " + childOrderField + " = CASE" +
                        " WHEN " + idField + " = @FirstLinkID THEN @s2" +
                        " WHEN " + idField + " = @SecondLinkID THEN @s1" + " END" +
                        " WHERE " + idField + " IN ( @FirstLinkID, @SecondLinkID)",
                        delegate(SqlCommand cmd)
                            {
                                AddParam(cmd, "@FirstLinkID", SqlDbType.Int, firstLinkID);
                                AddParam(cmd, "@SecondLinkID", SqlDbType.Int, secondLinkID);
                            });
        }

        public bool DelLink(int linkID)
        {
            return Exec(
                "DELETE FROM " + tableNameUpdDel + " WHERE " + idField + " = @LinkID",
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@LinkID", SqlDbType.Int, linkID);
                    });
        }

        public bool DelLink(int parentDocID, int childDocID)
        {
            return Exec(
                "DELETE FROM " + tableNameUpdDel + " WHERE " + idField + " = (SELECT TOP 1 " +
                idField + " FROM " + tableName + " WHERE " + parentDocIDField + " = @ParentDocID AND " + childDocIDField +
                " = @ChildDocID AND " + subFieldIDField + " IS NULL)",
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@ParentDocID", SqlDbType.Int, parentDocID);
                        AddParam(cmd, "@ChildDocID", SqlDbType.Int, childDocID);
                    });
        }

        public bool SendMessage(int firstLinkID, int secondLinkID, string message, int empID)
        {
            return Exec("declare @RecID varchar(300) " +
                        "create table #t1 (" + empIDField + " int, " + empField + " varchar(50), " + inWorkField +
                        " int) " +
                        "create table #t2 (" + empIDField + " int, " + empField + " varchar(50), " + inWorkField +
                        " int) " +
                        "insert into #t1 exec " + spDocRights + " @DocID " +
                        "insert into #t2 exec " + spDocRights + " @LinkDocID " +
                        "select @RecID = case when @RecID is null then '' else @RecID + ', ' END + convert(varchar(4),#t1." +
                        empIDField + ") from #t1 inner join #t2 on #t1." + empIDField + " = #t2." + empIDField +
                        " WHERE #t1." + empIDField + " <> @Kod AND #t1." + inWorkField + " = 1 " +
                        "if @RecID IS NOT null exec " + spSendMessage + " @DocID, @Message, @RecID SET @RecID = NULL " +
                        "select @RecID = case when @RecID is null then '' else @RecID + ', ' END + convert(varchar(4),#t1." +
                        empIDField + ") from #t1 inner join #t2 on #t1." + empIDField + " = #t2." + empIDField +
                        " WHERE #t1." + empIDField + " <> @Kod AND #t2." + inWorkField + " = 1 " +
                        "if @RecID IS NOT null exec " + spSendMessage + " @LinkDocID, @Message, @RecID",
                        delegate(SqlCommand cmd)
                            {
                                AddParam(cmd, "@Message", SqlDbType.NVarChar, message);
                                AddParam(cmd, "@DocID", SqlDbType.Int, firstLinkID);
                                AddParam(cmd, "@LinkDocID", SqlDbType.Int, secondLinkID);
                                AddParam(cmd, "@Kod", SqlDbType.Int, empID);
                            });
        }

        #endregion
    }
}