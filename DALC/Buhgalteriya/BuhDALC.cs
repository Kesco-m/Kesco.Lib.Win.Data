using System;
using System.Data;
using System.Data.SqlClient;
using Kesco.Lib.Win.Data.Business.V2.Corporate.DomainObjects;

namespace Kesco.Lib.Win.Data.DALC.Buhgalteriya
{
	/// <summary>
	/// попытка написать DALC для доступа к бухгалтерии
	/// </summary>
	public class BuhDALC : DALC
	{
		protected const string docType1CField = "КодТипа1СДокумента";
		protected const string docTypeIDField = "КодТипаДокумента";
		internal const string doc1CField = "Документ 1С";
		internal const string docField = "Документ ОперУчета";
		internal const string orgField = "Организация";
		internal const string typeBuhField = "ТипБухгалтерии";

		protected const string ruleField = "Роль";
		protected const string timeField = "ВремяУчёта";
		protected const string buhField = "Бухгалтерия";
		protected const string unUsedField = "НеИспользовать";

		internal const string viewName = "dbo.vwХозОперации";
		protected const string buhTableName = "dbo.vwБухгалтерии";
		protected const string buh1sTableName = "dbo.vwБухгалтерии1С";
		protected const string buhPersonIDField = "КодЛица";
		protected const string dbaseField = "Dbase";
		protected const string dbaseScalaField = "DbaseScala";
		protected const string typeField = "Тип";

		public BuhDALC(string connectionString) : base(connectionString)
		{
			tableName = "dbo.ХозОперации";
			idField = "КодХозОперации";
			nameField = "ХозОперация";
		}

		#region Accessors

		public string ViewName
		{
			get { return viewName; }
		}

		public string DocType1CField
		{
			get { return docType1CField; }
		}

		public string DocTypeIDField
		{
			get { return docTypeIDField; }
		}

		public string Doc1CField
		{
			get { return doc1CField; }
		}

		public string DocField
		{
			get { return docField; }
		}

		public string RuleField
		{
			get { return ruleField; }
		}

		public string TimeField
		{
			get { return timeField; }
		}

		public string OrgField
		{
			get { return orgField; }
		}

		public string UnUsedField
		{
			get { return unUsedField; }
		}

		public string BuhField
		{
			get { return buhField; }
		}

		public string BuhPersonIDField
		{
			get { return buhPersonIDField; }
		}

		public string DbaseField
		{
			get { return dbaseField; }
		}

		#endregion

		#region Get Data

		public int GetIDNext(int economicID)
		{
			return GetIdentityField(
				"SELECT MIN(" + idField + ") " + idField +
				" FROM " + tableName +
				" WHERE " + idField + " > @EconomicID",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@EconomicID", SqlDbType.Int, economicID);
				});
		}

		public int GetIDBefore(int economicID)
		{
			return GetIdentityField(
				"SELECT MAX(" + idField + ") " + idField +
				" FROM " + tableName +
				" WHERE " + idField + " < @EconomicID",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@EconomicID", SqlDbType.Int, economicID);
				});
		}

		public DataTable GetTransactonRules(int docTypeID)
		{
			return GetDataTable("SELECT DISTINCT " + ruleField + " FROM " + tableName + " WHERE " + docTypeIDField + " = @DocTypeID",
			   delegate(SqlCommand cmd)
			   {
				   AddParam(cmd, "@DocTypeID", SqlDbType.Int, docTypeID);
			   });
		}

		public DataTable GetTransacton(int docTypeID)
		{
			return GetTransacton(docTypeID, 0);
		}

		public DataTable GetTransacton(int docTypeID, int ruleID)
		{
			return GetDataTable("SELECT * " + " FROM " + tableName + " WHERE " + docTypeIDField + " = @DocTypeID" +
						  ((ruleID > 0) ? (" AND " + ruleField + " = @RuleID") : ""),
			   delegate(SqlCommand cmd)
			   {
				   AddParam(cmd, "@DocTypeID", SqlDbType.Int, docTypeID);

				   if(ruleID > 0)
					   AddParam(cmd, "@RuleID", SqlDbType.Int, ruleID);
			   });
		}

		public DataTable GetSimularEconomic(int type1CID, int typeOYID, int economicID)
		{
			return GetDataTable("SELECT " + idField + ", " + nameField + " FROM " + tableName + " WHERE " + docTypeIDField +
						" = @typeID AND " + docType1CField + " = @type1CID" +
						((economicID > 0) ? " AND " + idField + " <> @EconomicID" : ""),
			   delegate(SqlCommand cmd)
			   {
				   AddParam(cmd, "@typeID", SqlDbType.Int, typeOYID);
				   AddParam(cmd, "@type1CID", SqlDbType.Int, type1CID);
				   if(economicID > 0)
					   AddParam(cmd, "@EconomicID", SqlDbType.Int, economicID);
			   });
		}

		public DataRow GetEconomic(int econID)
		{
			return GetFirstRow("SELECT *" + " FROM " + viewName + ((econID > 0) ? " WHERE " + idField + " = @EconomicID" : ""),
			   delegate(SqlCommand cmd)
			   {
				   if(econID > 0)
					   AddParam(cmd, "@EconomicID", SqlDbType.Int, econID);

			   });
		}

		public DataTable GetAllEconomic()
		{
			return GetDataTable("SELECT * FROM " + viewName + " ORDER BY " + idField, null);
		}

		public bool IsBuh(int personID)
		{
			return GetIntField(
				"SELECT TOP 1 " + buhPersonIDField + " FROM " + buh1sTableName + " WITH (NOLOCK) WHERE " + buhPersonIDField + " = @PersonID AND " +
				typeField + " = 0",
				buhPersonIDField,
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@PersonID", SqlDbType.Int, personID);
				}) > 0;
		}

		public DataTable BuhPersons(string personIDs)
		{
			return GetDataTable("SELECT " + buhPersonIDField + ", " + buhField + ", " + dbaseField + " FROM " + buh1sTableName +
					  "  WITH (NOLOCK) WHERE " + buhPersonIDField + " IN (" + personIDs + ") AND " + typeField + " = 0", null);
		}

		public DataTable Get1CName()
		{
			return GetDataTable("SELECT " + buhPersonIDField + ", " + buhField + ", " + dbaseField + " FROM " + buh1sTableName +
								   " WITH (NOLOCK) WHERE " + typeField + " = 0", null);
		}

		public bool IsScala(int personID)
		{
			return GetIntField("SELECT TOP 1 " + buhPersonIDField +
							   " FROM " + buhTableName +
							   " WITH (NOLOCK) WHERE " + buhPersonIDField + " = @PersonID AND " +
							   dbaseScalaField + " <> ''",
							   buhPersonIDField,
							   delegate(SqlCommand cmd)
							   {
								   AddParam(cmd, "@PersonID", SqlDbType.Int, personID);
							   }) > 0;
		}

		public DataTable ScalaPersons(string personIDs)
		{
			string roles = Role._Аудитор + ", " + Role._Бухгалтер + ", " +
						   Role._ГлавБух1С + ", " + Role._Администратор1С;
			return GetDataTable("SELECT " + buhPersonIDField + ", " + buhField + " FROM dbo.Бухгалтерии " + " WHERE (" +
								   dbaseScalaField + " <> '') " + " AND EXISTS (SELECT * FROM Инвентаризация.dbo.fn_ТекущиеРоли() X " +
								   " WHERE КодРоли IN (" + roles + ") AND (КодЛица = 0 OR КодЛица = Бухгалтерии." + buhPersonIDField + ")) " +
								   " AND " + buhPersonIDField + " IN (" + personIDs + ")", null);
		}

		public bool IsIcFood(int personID)
		{
			return GetIntField("SELECT " +
							   "TOP 1 " + buhPersonIDField +
							   " FROM " + buh1sTableName +
							   " WITH (NOLOCK) WHERE " + buhPersonIDField + " = @PersonID AND " +
							   typeField + " = 1",
							   buhPersonIDField,
							   delegate(SqlCommand cmd)
							   {
								   AddParam(cmd, "@PersonID", SqlDbType.Int, personID);
							   }) > 0;
		}

		public DataTable BuhIcFoodPersons(string personIDs)
		{
			return GetDataTable("SELECT " + buhPersonIDField + ", " + buhField + " FROM " + buh1sTableName + " WHERE " +
									buhPersonIDField + " IN (" + personIDs + ") AND " + typeField + " = 1", null);
		}

		public DataTable GetTypes1C(int id)
		{
			return GetDataTable("SELECT DISTINCT " + docTypeIDField + " FROM " + tableName + " WHERE " + typeBuhField +
						" = @TypeBuh AND " + docTypeIDField + " IS NOT NULL",
						 delegate(SqlCommand cmd) { AddParam(cmd, "@TypeBuh", SqlDbType.Int, id); });
		}

		public DataTable Get1CTypes()
		{
			return GetTypes1C(0);
		}

		public DataTable Get1CFoodTypes()
		{
			return GetTypes1C(1);
		}

		#endregion

		#region Change Data

		public bool ChangeEconomic(int economicID, string name, int type1CID, int typeOYID, byte ruleID, string descr, int time, bool notUse)
		{
			return Exec(
				"UPDATE " + tableName +
				" SET " + nameField + " = @Economic, " +
				docType1CField + " = @Type1CID, " +
				docTypeIDField + " = @TypeID, " +
				ruleField + " = @RuleID, " +
				descriptionField + " = @Descr, " +
				timeField + " = @Time, " +
				unUsedField + " = @NotUse " +
				"WHERE " + idField + " = @ID",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@ID", SqlDbType.Int, economicID);
					AddParam(cmd, "@Economic", SqlDbType.VarChar, name);
					AddParam(cmd, "@Type1CID", SqlDbType.Int, type1CID);
					if(typeOYID > 0)
						AddParam(cmd, "@TypeID", SqlDbType.Int, typeOYID);
					else
						AddParam(cmd, "@TypeID", SqlDbType.Int, DBNull.Value);
					if(ruleID > 0)
						AddParam(cmd, "@RuleID", SqlDbType.TinyInt, ruleID);
					else
						AddParam(cmd, "@RuleID", SqlDbType.TinyInt, DBNull.Value);
					AddParam(cmd, "@Descr", SqlDbType.NVarChar, descr);
					AddParam(cmd, "@Time", SqlDbType.Int, time);
					AddParam(cmd, "@NotUse", SqlDbType.TinyInt, notUse);
				});
		}

		#endregion
	}
}
