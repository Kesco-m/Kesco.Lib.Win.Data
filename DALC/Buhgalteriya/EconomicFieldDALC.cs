using System;
using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Buhgalteriya
{
	public class EconomicFieldDALC : DALC
	{
		internal string economicIDField = "КодХозОперации";
		internal string economicField = "ПолеХозОперации";
		internal string tablePart1CField = "ТабличнаяЧасть1С";
		internal string docField = "ПолеДокумента";
		internal string viewName = "dbo.vwПоляХозОпераций";
		internal string spFieldOY = "dbo.sp_ПоляОУдляХозоперации";

		public EconomicFieldDALC(string connectionString)
			: base(connectionString)
		{
			tableName = "dbo.ПоляХозОпераций";
			idField = "КодПоляХозОперации";
		}

		#region Accessories

		public string EconomicIDField
		{
			get { return economicIDField; }
		}

		public string EconomicField
		{
			get { return economicField; }
		}

		public string TablePart1CField
		{
			get { return tablePart1CField; }
		}


		public string DocField
		{
			get { return docField; }
		}

		#endregion

		#region Get Data

		public DataSet GetEconomicFields(int economicID)
		{
			return GetData(
				"SELECT * " +
				"FROM " + viewName +
				" WHERE " + economicIDField + " = @EconomicIDField",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@EconomicIDField", SqlDbType.Int, economicID);
				});
		}

		public DataRow GetField1(int id)
		{
			return GetFirstRow(
				"SELECT * " +
				"FROM " + viewName +
				" WHERE " + idField + " = @IDField",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@IDField", SqlDbType.Int, id);
				});
		}

		public DataSet GetEconFields(int docTypeID)
		{
			return GetData(
				spFieldOY,
				delegate(SqlCommand cmd)
				{
					cmd.CommandType = CommandType.StoredProcedure;
					AddParam(cmd, "@КодТипаДокумента", SqlDbType.Int, docTypeID);
				});
		}
		#endregion

		#region Change Data

		public bool AddEconomicField(int econID, int field1CID, byte buhPar, int fieldID, string choiseStr, string constnt, string descr)
		{
			return Exec(
				"INSERT INTO " + tableName + " " +
				"(КодХозОперации, КодПоля1С, КодПоляОУ, БухПараметр, Выбор, Константа, " + descriptionField + ") " +
				"VALUES ( @EconomicID, @Field1cID, @FieldID, @BuhType, @ChoiseStr, @ConstantStr, @Descr)",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@EconomicID", SqlDbType.Int, econID);
					AddParam(cmd, "@Field1cID", SqlDbType.Int, field1CID);
					if (fieldID > 0)
						AddParam(cmd, "@FieldID", SqlDbType.Int, fieldID);
					else
						AddParam(cmd, "@FieldID", SqlDbType.Int, DBNull.Value);
					AddParam(cmd, "@BuhType", SqlDbType.TinyInt, buhPar);
					AddParam(cmd, "@ChoiseStr", SqlDbType.VarChar, choiseStr);
					AddParam(cmd, "@ConstantStr", SqlDbType.VarChar, constnt);
					AddParam(cmd, "@Descr", SqlDbType.VarChar, descr);
				});
		}

		public bool ChangeEconomicField(int economicFeildID, byte buhPar, int fieldID, string choiseStr, string constnt, string descr)
		{
			return Exec(
				"UPDATE " + tableName + " " +
				"SET КодПоляОУ = @FieldID, БухПараметр = @BuhType, Выбор = @ChoiseStr, Константа = @ConstantStr, " + descriptionField + " = @Descr " +
				"WHERE (" + idField + " = @EconomicFieldID)",
			delegate(SqlCommand cmd)
			{
				AddParam(cmd, "@EconomicFieldID", SqlDbType.Int, economicFeildID);
				if (fieldID > 0)
					AddParam(cmd, "@FieldID", SqlDbType.Int, fieldID);
				else
					AddParam(cmd, "@FieldID", SqlDbType.Int, DBNull.Value);
				AddParam(cmd, "@BuhType", SqlDbType.TinyInt, buhPar);
				AddParam(cmd, "@ChoiseStr", SqlDbType.VarChar, choiseStr);
				AddParam(cmd, "@ConstantStr", SqlDbType.VarChar, constnt);
				AddParam(cmd, "@Descr", SqlDbType.VarChar, descr);
			});
		}

		public bool SetEconomicFieldFromEconomic(int setEconomicID, int baseEconomicID)
		{
			return Exec(
				" BEGIN TRAN T1 " +
				" DELETE FROM " + tableName +
				" WHERE КодХозОперации = @SetID " +
				" INSERT INTO " + tableName +
				" (КодХозОперации, КодПоля1С, КодПоляОУ, БухПараметр, Выбор, Константа, " + descriptionField + ") " +
				" SELECT @SetID, КодПоля1С, КодПоляОУ, БухПараметр, Выбор, Константа, " + descriptionField +
				" FROM " + tableName +
				" WHERE КодХозОперации = @BaseID" +
				" COMMIT TRAN T1",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@SetID", SqlDbType.Int, setEconomicID);
					AddParam(cmd, "@BaseID", SqlDbType.Int, baseEconomicID);
				});
		}

		#endregion
	}
}