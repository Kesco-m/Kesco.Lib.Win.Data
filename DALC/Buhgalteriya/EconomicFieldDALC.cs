using System;
using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Buhgalteriya
{
	public class EconomicFieldDALC : DALC
	{
		internal string economicIDField = "��������������";
		internal string economicField = "���������������";
		internal string tablePart1CField = "��������������1�";
		internal string docField = "�������������";
		internal string viewName = "dbo.vw���������������";
		internal string spFieldOY = "dbo.sp_��������������������";

		public EconomicFieldDALC(string connectionString)
			: base(connectionString)
		{
			tableName = "dbo.���������������";
			idField = "������������������";
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
					AddParam(cmd, "@����������������", SqlDbType.Int, docTypeID);
				});
		}
		#endregion

		#region Change Data

		public bool AddEconomicField(int econID, int field1CID, byte buhPar, int fieldID, string choiseStr, string constnt, string descr)
		{
			return Exec(
				"INSERT INTO " + tableName + " " +
				"(��������������, �������1�, ���������, �����������, �����, ���������, " + descriptionField + ") " +
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
				"SET ��������� = @FieldID, ����������� = @BuhType, ����� = @ChoiseStr, ��������� = @ConstantStr, " + descriptionField + " = @Descr " +
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
				" WHERE �������������� = @SetID " +
				" INSERT INTO " + tableName +
				" (��������������, �������1�, ���������, �����������, �����, ���������, " + descriptionField + ") " +
				" SELECT @SetID, �������1�, ���������, �����������, �����, ���������, " + descriptionField +
				" FROM " + tableName +
				" WHERE �������������� = @BaseID" +
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