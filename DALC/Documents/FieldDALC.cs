using System.Data;
using System.Data.SqlClient;
using Kesco.Lib.Win.Data.Documents;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
	/// <summary>
	/// DAL-компонент для доступа к таблице Документы.dbo.ПоляДокументов
	/// </summary>
	public class FieldDALC : DALC
	{
        private const string docTypeIDField = "КодТипаДокумента";
        private const string fieldNameEngField = "ПолеДокументаEN";
        private const string fieldNameEtField = "ПолеДокументаET";
        private const string positionField = "ПорядокПоляДокумента";
        private const string tableColumnField = "КолонкаТаблицы";
        private const string fieldTypeIDField = "КодТипаПоля";
        private const string needField = "Обязательность";
        private const string digitCountField = "ЧислоДесятичныхЗнаков";
        private const string urlField = "URLПоиска";
        private const string searchParamsField = "ПараметрыПоиска";
        private const string connectionStringField = "СтрокаПодключения";
        private const string dataSourceField = "SQLЗапрос";
        private const string substFormTitleField = "ЗаголовокФормыПоиска";
        private const string multiSelectField = "МножественныйВыбор";
        private const string strongSearchField = "СтрогийПоиск";
        private const string searchTypeField = "РежимПоискаТипов";

		private DocumentDALC docData;
		private string docTable;
		private string docIDField;

		private FieldTypeDALC fieldTypeData;
		private string fieldTypeTableName;
		private string fieldTypeNameField;


		public FieldDALC(string connectionString) : base(connectionString)
		{
			tableName = "Документы.dbo.ПоляДокументов";

			idField = "КодПоляДокумента";
			nameField = "ПолеДокумента";
			
			docData = new DocumentDALC(connectionString);
			docTable = docData.TableName;
			docIDField = docData.IDField;

			fieldTypeData = new FieldTypeDALC( connectionString);
			fieldTypeTableName = fieldTypeData.TableName;
			fieldTypeNameField = fieldTypeData.FieldNameField;
		}

		#region Accessors

		public string DocTypeIDField
		{
			get { return docTypeIDField; }
		}

        public string FieldNameEngField
        {
            get { return fieldNameEngField; }
        }

        public string FieldNameEtField
        {
            get { return fieldNameEtField; }
        }

		public string PositionField
		{
			get { return positionField; }
		}

		public string TableColumnField
		{
			get { return tableColumnField; }
		}

		public string FieldTypeIDField
		{
			get { return fieldTypeIDField; }
		}

		public string NeedField
		{
			get { return needField; }
		}

		public string DigitCountField
		{
			get { return digitCountField; }
		}

		public string URLField
		{
			get { return urlField; }
		}

		public string SearchParamsField
		{
			get { return searchParamsField; }
		}

		public string ConnectionStringField
		{
			get { return connectionStringField; }
		}

		public string DataSourceField
		{
			get { return dataSourceField; }
		}

		public string SubstFormTitleField
		{
			get { return substFormTitleField; }
		}

		public string MultiSelectField
		{
			get { return multiSelectField; }
		}

		public string StrongSearchField
		{
			get { return strongSearchField; }
		}

		public string SearchTypeField
		{
			get { return searchTypeField; }
		}

		#endregion

		#region Get Data

		public DataSet GetFieldType(int docTypeID)
		{
			var cmd = new SqlDataAdapter(
				"SELECT SUBSTRING(" + tableColumnField + ", 8, 1) " + tableColumnField + ", " + 
				nameField +
				" FROM " + tableName +
				" WHERE " + docTypeIDField + "= @DocTypeID AND " + tableColumnField + " LIKE 'КодЛица%' " +
				"ORDER BY SUBSTRING(" + tableColumnField + ", 8, 1)",
				new SqlConnection( connectionString));

			AddParam(cmd.SelectCommand, "@DocTypeID", SqlDbType.Int, docTypeID);

			return CMD_FillDS(cmd);
		}

		public DataSet GetFields(int docTypeID)
		{
			return GetData("SELECT " +
				idField + ", " +
				nameField + ", " +
				fieldNameEngField + ", " +
				fieldNameEtField + ", " +
				docTypeIDField + ", " +
				positionField + ", " +
				tableColumnField + ", " +
				tableName + "." + fieldTypeIDField + ", " +
				fieldTypeNameField + ", " +
				needField + ", " +
				digitCountField + ", " +
				urlField + ", " +
				searchParamsField + ", " +
				connectionStringField + ", " +
				dataSourceField + ", " +
				substFormTitleField + ", " +
				descriptionField + ", " +
				multiSelectField + ", " +
				searchTypeField + ", " +
				strongSearchField + ", " +
				editorField + ", " +
				editedField +
				" FROM " + tableName + " INNER JOIN " + fieldTypeTableName +
				" ON " + tableName + "." + fieldTypeIDField + " = " + fieldTypeTableName + "." + fieldTypeIDField +
				" WHERE " + docTypeIDField + " = @DocTypeID" +
				" ORDER BY " + positionField,
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@DocTypeID", SqlDbType.Int, docTypeID);
				});
		}

		#endregion

		#region Change Data

		public int Add(
            string name,
            string nameEng,
            string nameEt,
			int docTypeID,
			int position,
			string tableColumn,
			int fieldTypeID,
			bool need,
			int digitCount,
			string url,
			string searchParams,
			string conString,
			string dataSource,
			string substFormTitle,
			string description,
			int searchType,
			bool strongSearch,
			bool multiselect)
		{
			return GetIdentityField(
				"INSERT INTO " + tableName +
				" (" +
					nameField + ", " +
                    fieldNameEngField + ", " +
                    fieldNameEtField + ", " +
					docTypeIDField + ", " +
					positionField + ", " +
					tableColumnField + ", " +
					fieldTypeIDField + ", " +
					needField + ", " +
					digitCountField + ", " +
					urlField + ", " +
					searchParamsField + ", " +
					connectionStringField + ", " +
					dataSourceField + ", " +
					substFormTitleField + ", " +
					descriptionField + ", " +
					searchTypeField + ", " +
					strongSearchField + ", " +
					multiSelectField +
                ")\n VALUES (@Name, @NameEng, @NameEt, @DocTypeID, @Position, @TableColumn, @FieldTypeID, @Need, " +
				"@DigitCount, @URL, @SearchParams, @ConnectionString, @DataSource, @SubstFormTitle, @Description, @SearchType, @StrongSearch, @MultiSelect)\n" +
				" SELECT SCOPE_IDENTITY() AS " + identityField,
				delegate(SqlCommand cmd)
				{
                    AddParam(cmd, "@Name", SqlDbType.NVarChar, name);
                    AddParam(cmd, "@NameEng", SqlDbType.VarChar, nameEng);
                    AddParam(cmd, "@NameEt", SqlDbType.VarChar, nameEt);
					AddParam(cmd, "@DocTypeID", SqlDbType.Int, docTypeID);
					AddParam(cmd, "@Position", SqlDbType.Int, position);
					AddParam(cmd, "@TableColumn", SqlDbType.VarChar, tableColumn);
					AddParam(cmd, "@FieldTypeID", SqlDbType.Int, fieldTypeID);
					AddParam(cmd, "@Need", SqlDbType.Bit, (need ? 1 : 0));
					AddParam(cmd, "@DigitCount", SqlDbType.Int, digitCount);
					AddParam(cmd, "@URL", SqlDbType.VarChar, url);
					AddParam(cmd, "@SearchParams", SqlDbType.VarChar, searchParams);
					AddParam(cmd, "@ConnectionString", SqlDbType.VarChar, conString);
					AddParam(cmd, "@DataSource", SqlDbType.NVarChar, dataSource);
					AddParam(cmd, "@SubstFormTitle", SqlDbType.VarChar, substFormTitle);
					AddParam(cmd, "@Description", SqlDbType.NVarChar, description);
					AddParam(cmd, "@SearchType", SqlDbType.TinyInt, searchType);
					AddParam(cmd, "@StrongSearch", SqlDbType.Bit, strongSearch);
					AddParam(cmd, "@MultiSelect", SqlDbType.Bit, multiselect);
				});
		}

		public bool Save(
			int id,
            string name,
            string nameEng,
            string nameEt,
			int docTypeID,
			int position,
			string tableColumn,
			int fieldTypeID,
			bool need,
			int digitCount,
			string url,
			string searchParams,
			string conString,
			string dataSource,
			string substFormTitle,
			string description,
			int searchType,
			bool strongSearch,
			bool multiselect)
		{
			var cmd = new SqlCommand(
				"UPDATE " + tableName +
				" SET " + 
					nameField + " = @Name, " +
                    fieldNameEngField + " = @NameEng, " +
                    fieldNameEtField + " = @NameEt," +
					docTypeIDField + " = @DocTypeID, " +
					positionField + " = @Position, " +
					tableColumnField + " = @TableColumn, " +
					fieldTypeIDField + " = @FieldTypeID, " +
					needField + " = @Need, " +
					digitCountField + " = @DigitCount, " +
					urlField + " = @URL, " +
					searchParamsField + " = @SearchParams, " +
					connectionStringField + " = @ConnectionString, " +
					dataSourceField + " = @DataSource, " +
					substFormTitleField + " = @SubstFormTitle, " +
					descriptionField + " = @Description, " +
					searchTypeField + " = @SearchType, " +
					strongSearchField + " = @StrongSearch, " +
					multiSelectField + " = @MultiSelect" +
				" WHERE " + idField + " = @ID",
				new SqlConnection(connectionString));

            AddParam(cmd, "@Name", SqlDbType.NVarChar, name);
            AddParam(cmd, "@NameEng", SqlDbType.VarChar, nameEng);
            AddParam(cmd, "@NameEt", SqlDbType.VarChar, nameEt);
			AddParam(cmd, "@DocTypeID", SqlDbType.Int, docTypeID);
			AddParam(cmd, "@Position", SqlDbType.Int, position);
			AddParam(cmd, "@TableColumn", SqlDbType.VarChar, tableColumn);
			AddParam(cmd, "@FieldTypeID", SqlDbType.Int, fieldTypeID);
			AddParam(cmd, "@Need", SqlDbType.Bit, (need ? 1 : 0));
			AddParam(cmd, "@DigitCount", SqlDbType.Int, digitCount);
			AddParam(cmd, "@URL", SqlDbType.VarChar, url);
			AddParam(cmd, "@SearchParams", SqlDbType.VarChar, searchParams);
			AddParam(cmd, "@ConnectionString", SqlDbType.VarChar, conString);
			AddParam(cmd, "@DataSource", SqlDbType.VarChar, dataSource);
			AddParam(cmd, "@SubstFormTitle", SqlDbType.VarChar, substFormTitle);
			AddParam(cmd, "@Description", SqlDbType.VarChar, description);
			AddParam(cmd, "@SearchType", SqlDbType.TinyInt, searchType);
			AddParam(cmd, "@StrongSearch", SqlDbType.Bit, strongSearch);
			AddParam(cmd, "@MultiSelect", SqlDbType.Bit, multiselect);
			AddParam(cmd, "@ID", SqlDbType.Int, id);

			return CMD_Exec(cmd);
		}

		#endregion
	}
}