using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    /// <summary>
    /// DAL-компонент для доступа к таблице Документы.dbo.ТипыДокументов
    /// </summary>
	public class DocTypeDALC : TreeDALC
	{
		private const string typeDocField = "TypeDoc";
		private const string formPresentField = "НаличиеФормы";
		private const string viewNameField = "ИмяПредставления";
		private const string urlField = "URL";
		private const string searchURLField = "SearchURL";
		private const string addNameField = "ТОписание";
		private const string addNameEngField = "TDescription";
		private const string helpURLField = "HelpURL";
		private const string outGoingField = "Исходящий";
		private const string buhField = "Бухгалтерский";
		private const string buhDirectoryField = "БухгалтерскийСправочник";
		private const string numberAutogenTypeField = "ТипАвтогенерацииНомера";
		private const string numberAutogenAlgorithmField = "АлгоритмАвтогенерацииНомера";
		private const string finansField = "Финансовый";
		private const string protectedField = "СоздаватьЗащищеным";
		private const string answerFormField = "ТипОтвета";
		private const string nameExistField = "NameExist";

		private string nameRLField;

		private const string synFunc = "dbo.fn_СинонимыТиповДокументов";

		private string selectString;
		private string orderString;

		private const string searchTypeField = "УсловиеПохожести";

        public const int TYPE_ID_SCHET = 2283;
        public const int TYPE_ID_SCHET_FACTURA = 2070;
        public const int TYPE_ID_INVOICE = 2071;
        public const int TYPE_ID_INVOICE_PROFORMA = 2284;

		public DocTypeDALC(string connectionString) : base(connectionString)
		{
			tableName = "ТипыДокументов";

			idField = "КодТипаДокумента";
			nameField = "ТипДокумента";

			nameRLField = nameField + "RL";

			selectString =
				"SELECT " +
				idField + ", " +
				nameField + " + ' ' + " + addNameField + " " + nameField + ", " +
				nameField + " " + NameFieldNative + ", " +
				addNameField + ", " +
				parentField + ", " +
				typeDocField + " + ' ' + " + addNameEngField + " " + typeDocField + ", " +
				typeDocField + " " + TypeDocFieldNative + ", " +
				addNameEngField + ", " +
				formPresentField + ", " +
				outGoingField + ", " +
				buhField + ", " +
				buhDirectoryField + ", " +
				finansField + ", " +
				numberAutogenTypeField + ", " +
				numberAutogenAlgorithmField + ", " +
				viewNameField + ", " +
				urlField + ", " +
				searchURLField + ", " +
				helpURLField + ", " +
				answerFormField + ", " +
				protectedField + ", " +
				searchTypeField + ", " +
				"Инвентаризация.dbo.fn_ReplaceRusLat(" + nameField + ") AS " + nameRLField + ", " +
				editorField + ", " +
				editedField + ", " +
				nameExistField +
				" FROM " + tableName;

			orderString = " ORDER BY L";
		}

		#region Accessors

		public string TypeDocField
		{
			get { return typeDocField; }
		}

		public string TypeDocFieldNative
		{
			get { return typeDocField + "Native"; }
		}

		public string NameFieldNative
		{
			get { return nameField + "Native"; }
		}

		/// <summary>
		/// Поле, означающее наличие названия у документа
		/// </summary>
		public string NameExistField
		{
			get { return nameExistField; }
		}

		public string FormPresentField
		{
			get { return formPresentField; }
		}

		public string ViewNameField
		{
			get { return viewNameField; }
		}

		public string URLField
		{
			get { return urlField; }
		}

		public string SearchURLField
		{
			get { return searchURLField; }
		}

		public string HelpURLField
		{
			get { return helpURLField; }
		}

		public string AddNameField
		{
			get { return addNameField; }
		}

		public string AddNameEngField
		{
			get { return addNameEngField; }
		}

		public string NumberAutogenTypeField
		{
			get { return numberAutogenTypeField; }
		}

		public string NumberAutogenAlgorithmField
		{
			get { return numberAutogenAlgorithmField; }
		}

		public string NameRLField
		{
			get { return nameRLField; }
		}

		public string OutGoingField
		{
			get { return outGoingField; }
		}

		public string BuhField
		{
			get { return buhField; }
		}

		public string BuhDirectoryField
		{
			get { return buhDirectoryField; }
		}

		public string FinansField
		{
			get { return finansField; }
		}

		public string AnswerFormField
		{
			get { return answerFormField; }
		}

		public string ProtectedField
		{
			get { return protectedField; }
		}

		public string SearchTypeField
		{
			get { return searchTypeField; }
		}

		#endregion

		#region Get Data

		public DataTable GetDocTypes()
		{
			return GetDocTypes(false);
		}

		public DataTable GetDocTypes(bool formNeeded)
		{
			string where = "";
			if(formNeeded)
				where = " WHERE " + formPresentField + " > 0";

			using(
				var cmd = new SqlDataAdapter(selectString + where + orderString, new SqlConnection(connectionString)))
			{
				return CMD_FillDT(cmd.SelectCommand);
			}
		}

		public DataSet GetDocTreeTypes()
		{
			return GetTreeData(this.selectString + this.orderString, null);
		}


		public DataSet GetDocPrintTypes()
		{
			string where = " WHERE EXISTS (SELECT * FROM vwПечатныеФормы WHERE " + idField + " = " + tableName + "." + idField + ") ";

			return GetTreeData(selectString + where + orderString, null);
		}

		public DataTable GetDocAnswerTypes()
		{
			const string where = " WHERE " + formPresentField + " > 0";

			return GetDataTable("SELECT 0 " + idField + ", 'Нет' " + nameField + " UNION " +
				" SELECT " + idField + ", " + nameField + " FROM " + tableName + " (nolock) " + where, null);
		}

		public DataSet GetBuhDocTypes()
		{
			const string where = " WHERE " + buhField + " = 1";

			return GetTreeData(selectString + where + " ORDER BY " + nameField, null);
		}

		public DataTable GetBuhDirectoryDocTypes()
		{
			return GetDataTable("SELECT " + idField + " FROM " + tableName + " WHERE " + buhDirectoryField + " > 0", null);
		}

		public DataSet GetAutogenDocTypes()
		{
			const string where = " WHERE " + numberAutogenTypeField + " in ( 1, 2)";

			return GetTreeData(selectString + where + " ORDER BY " + nameField, null);
		}

		public DataSet GetChildDocTypes(int parentID, bool formNeeded)
		{
			string where = "";
			if(formNeeded)
				where = " AND " + formPresentField + " > 0";

			return GetData(selectString + " WHERE " + parentField + " = @ParentID" + where + orderString,
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@ParentID", SqlDbType.Int, parentID);
				});
		}


		public DataSet GetTypesLike(string typeLike)
		{
			return GetData("SELECT " + idField + " FROM " + tableName +
					" WHERE Инвентаризация.dbo.fn_ReplaceRusLat(" + nameField + ") LIKE '" + Replacer.ReplaceLike(typeLike) + "%'", null);
		}

		public DataSet GetChildDocTypes(int parentID)
		{
			return GetChildDocTypes(parentID, false);
		}

		public DataSet GetSynonymDocTypes(int id, bool formNeeded, bool synonimus)
		{
			string where = "";
			if(formNeeded)
				where = " AND " + formPresentField + " > 0";

			return GetData(selectString + " WHERE " + parentField + " = (" +
					(synonimus ? "SELECT Parent." + idField + " FROM " + tableName + " Child" +
						   " LEFT JOIN " + tableName + " Parent" + " ON Parent." + idField + " = Child." + parentField +
						   " WHERE Child." + idField + " = @ID AND Child." + parentField + " IS NOT NULL AND Parent." +
						   nameField + " IS NULL"
						 : "SELECT " + parentField + " FROM " + tableName + " WHERE " + idField + " = @ID") +
					") AND Parent IS NOT NULL AND " + idField + " <> @ID" +
					where +
					orderString,
					delegate(SqlCommand cmd)
					{
						AddParam(cmd, "@ID", SqlDbType.Int, id);
					});
		}

		public string GetDocType(int id, string lang)
		{
			var ret = GetRecord<string>("SELECT " + (lang.Equals("ru") ? nameField : typeDocField) + " " + nameField +
				" FROM " + tableName + " WHERE " + idField + " = @ID",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@ID", SqlDbType.Int, id);
				},
				dr => dr[0] is string ? dr[0] as string : (lang.StartsWith("ru") ? "Не известен" : "No type"));
			if(string.IsNullOrEmpty(ret))
				throw new Exception("Не найден тип с кодом " + id);
			return ret;
		}

		public bool IsParentSynonymGroup(int id)
		{
			return GetField("SELECT " +
							"Parent." + nameField +
							" FROM " + tableName + " Child" +
							" LEFT JOIN " + tableName + " Parent" +
							" ON Parent." + idField + " = Child." + parentField +
							" WHERE Child." + idField + " = @ID",
							nameField,
							delegate(SqlCommand cmd)
							{
								AddParam(cmd, "@ID", SqlDbType.Int, id);
							}) is string;
		}

		public List<int> GetContarcts(int typeID)
		{
			return GetRecords<int>("SELECT КодДокументаДоговораОказанияУслуг FROM vwДокументыДоговораОказанияУслуг WHERE КодТипаДокументаОснованияАвтооплаты = @TypeID", delegate(SqlCommand cmd)
			{
				AddParam(cmd, "@TypeID", SqlDbType.Int, typeID);
			}, null);
		}

		public int GetSlaveType(int docID)
		{
			return GetRecord<int>(@"SELECT	TOP 1 АС.КодДокументаДоговораОказанияУслуг
FROM	dbo.vwСвязиДокументов INNER JOIN vwДокументыДоговораОказанияУслуг АС ON dbo.vwСвязиДокументов.КодДокументаОснования = АС.КодДокументаДоговораОказанияУслуг
		 INNER JOIN dbo.vwДокументы ON АС.КодТипаДокументаОснованияАвтооплаты = dbo.vwДокументы.КодТипаДокумента AND 
			dbo.vwСвязиДокументов.КодДокументаВытекающего = dbo.vwДокументы.КодДокумента
WHERE	vwДокументы.КодДокумента = @DocID", delegate(SqlCommand cmd)
										  {
											  AddParam(cmd, "@DocID", SqlDbType.Int, docID);
										  }, delegate(IDataRecord dr)
										  {
											  if(dr == null && dr.IsDBNull(0) && !(dr[0] is int))
												  return 0;
											  return dr.GetInt32(0);
										  });
		}

		#endregion

		#region Change Data

		public bool SetProperties(int id, string addTypeName, string docTypeName, string addTypeEngName,
								  byte formPresent, bool inOut, bool buh, int buhDirectory, int finans,
								  int numberAutogenTypeID, int numberAutogenAlgorithmID, string viewName, string url,
								  string searchURL, string helpURL, bool makeProtected, int answerForm, byte searchType)
		{
			return Exec("UPDATE " + tableName + " SET " + addNameField + " = @AddTypeName, " +
					typeDocField + " = @TypeDoc, " + addNameEngField + " = @AddTypeEngName, " + formPresentField +
					" = @FormPresent, " + outGoingField + " = @InOut, " + buhField + " = @Buh, " + buhDirectoryField +
					" = @BuhDirectory, " + finansField + " = @Finans, " + numberAutogenTypeField +
					" = @NumberAutogenTypeID, " + numberAutogenAlgorithmField + " = @NumberAutogenAlgorithm, " +
					viewNameField + " = @ViewName, " + urlField + " = @URL, " + searchURLField + " = @SearchURL, " +
					helpURLField + " = @HelpURL, " + protectedField + " = @ProtectedField, " + answerFormField +
					((answerForm > 0) ? " = @AnswerForm, " : " = NULL, ") + searchTypeField + " = @SearchType" +
					" WHERE " + idField + " = @ID",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@AddTypeName", SqlDbType.NVarChar, addTypeName);
					AddParam(cmd, "@TypeDoc", SqlDbType.VarChar, docTypeName);
					AddParam(cmd, "@AddTypeEngName", SqlDbType.VarChar, addTypeEngName);
					AddParam(cmd, "@FormPresent", SqlDbType.TinyInt, formPresent);
					AddParam(cmd, "@InOut", SqlDbType.Bit, (inOut ? 1 : 0));
					AddParam(cmd, "@Buh", SqlDbType.Bit, (buh ? 1 : 0));
					if(buhDirectory > 0)
						AddParam(cmd, "@BuhDirectory", SqlDbType.Int, buhDirectory);
					else
						AddParam(cmd, "@BuhDirectory", SqlDbType.Int, DBNull.Value);
					AddParam(cmd, "@Finans", SqlDbType.Int, finans);
					AddParam(cmd, "@NumberAutogenTypeID", SqlDbType.Int, numberAutogenTypeID);
					AddParam(cmd, "@NumberAutogenAlgorithm", SqlDbType.Int, numberAutogenAlgorithmID);
					AddParam(cmd, "@ViewName", SqlDbType.VarChar, viewName);
					AddParam(cmd, "@URL", SqlDbType.VarChar, url);
					AddParam(cmd, "@SearchURL", SqlDbType.VarChar, searchURL);
					AddParam(cmd, "@HelpURL", SqlDbType.VarChar, helpURL);
					AddParam(cmd, "@ProtectedField", SqlDbType.Bit, (makeProtected ? 1 : 0));
					if(answerForm > 0)
						AddParam(cmd, "@AnswerForm", SqlDbType.Int, answerForm);
					AddParam(cmd, "@ID", SqlDbType.Int, id);
					AddParam(cmd, "@SearchType", SqlDbType.TinyInt, searchType);

				});
		}

		public bool New(int id, string text)
		{
			if(string.IsNullOrEmpty(text) && id == 0)
				return false;
			return Exec("INSERT INTO " + tableName + " (" + ((text != null) ? nameField : "") + ((id != 0) ? ((text != null) ? ", " : "") + parentField : "") + ")\n VALUES ("
					+ ((text != null) ? "@Name" : "") + ((id != 0) ? (((text != null) ? ", " : "") + "@ParentID") : "") + ")",
				delegate(SqlCommand cmd)
				{
					if(text != null)
						AddParam(cmd, "@Name", SqlDbType.NVarChar, text);

					if(id != 0)
						AddParam(cmd, "@ParentID", SqlDbType.Int, id);
				});
		}

		public override bool Delete(int id)
		{
			using(var tds = GetChildDocTypes(id))
			{
				if(tds.Tables[tableName].Rows.Count == 0) // нет подтипов у данного типа, можно удалять
					return base.Delete(id);
			}

			ErrorMessage("Тип не может быть удален, так как он содержит подтипы", false, "Отказано в удалении");
			return false;
		}

		public bool Move(int what, int where)
		{
			return Exec("UPDATE " + tableName + " SET " + parentField + " = @ParentID" + " WHERE " + idField + " = @ID",
			   delegate(SqlCommand cmd)
			   {
				   AddParam(cmd, "@ParentID", SqlDbType.Int, where);
				   AddParam(cmd, "@ID", SqlDbType.Int, what);
			   });
		}

		public bool SortedMove(int what, int before)
		{
			return Exec("UPDATE " + tableName + " SET " + parentField + " = (" + "SELECT " + parentField + " FROM " +
						 tableName + " WHERE " + idField + " = @BeforeID" + "), " + leftField + " = (" + "SELECT " +
						 leftField + " FROM " + tableName + " WHERE " + idField + " = @BeforeID)" + " WHERE " + idField + " = @ID",
						 delegate(SqlCommand cmd)
						 {
							 AddParam(cmd, "@BeforeID", SqlDbType.Int, before);
							 AddParam(cmd, "@ID", SqlDbType.Int, what);
						 });
		}

		#endregion
	}
}