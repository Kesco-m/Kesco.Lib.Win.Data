using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
	/// <summary>
	/// DAL-��������� ��� �������� �� ���������� � 1�
	/// </summary>
	public class BuhParamDocDALC : DALC
	{
		protected string buhParamContrTableName = " dbo.���������������������";
		protected string idDocField = "������������";
		protected string idBuhField = "��������������";
		protected string typeBaseField = "�������";
		protected string idBuhParamContr = "�����������������������";

		public BuhParamDocDALC(string connectionString)
			: base(connectionString)
		{
			tableName = "dbo.���������������������";
			idField = "������������������������";
		}

		#region Accessors

		public string BuhParamContrTableName
		{
			get { return buhParamContrTableName; }
		}

		public string TypeBaseField
		{
			get { return typeBaseField; }
		}

		public string IDBuhField
		{
			get { return idBuhField; }
		}

		#endregion

		#region GetData

		public bool IsSentDocToIc(int idDoc, int idBuh, int typeBase)
		{
			return GetIntField(
				"SELECT " +
				"TOP 1 " + idDocField +
				" FROM " + tableName +
				" WITH (NOLOCK) WHERE " + idDocField + " = @IdDoc AND " + idBuhField + " = @IdBuh " +
				((typeBase > -1) ? "AND " + typeBaseField + " = @TypeBase " : "") +
				" UNION " +
				"SELECT " +
				"TOP 1 " + idDocField +
				" FROM " + buhParamContrTableName +
				" WITH (NOLOCK) WHERE " + idDocField + " = @IdDoc AND " + idBuhField + " = @IdBuh" +
				((typeBase > -1) ? " AND " + typeBaseField + " = @TypeBase" : ""),
				idDocField,
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@IdDoc", SqlDbType.Int, idDoc);
					AddParam(cmd, "@IdBuh", SqlDbType.Int, idBuh);
					AddParam(cmd, "@TypeBase", SqlDbType.Int, typeBase);
				}) > 0;
		}

		public string GetSentDocToIcString(int idDoc, int idBuh)
		{
			return GetRecord<string>(
				"SELECT CASE WHEN EXISTS (SELECT * " +
				" FROM " + tableName +
				" WITH (NOLOCK) WHERE " + idDocField + " = @IdDoc " +
				((idBuh > 0) ? "AND " + idBuhField + " = @IdBuh " : "") +
				"AND " + typeBaseField + " = 0) " +
				" OR " +
				"EXISTS (SELECT *" +
				" FROM " + buhParamContrTableName +
				" WITH (NOLOCK) WHERE " + idDocField + " = @IdDoc " +
				((idBuh > 0) ? "AND " + idBuhField + " = @IdBuh " : "") +
				"AND " + typeBaseField + " = 0) THEN '�' ELSE '' END + " +
				"CASE WHEN EXISTS (SELECT * " +
				" FROM " + tableName +
				" WITH (NOLOCK) WHERE " + idDocField + " = @IdDoc " +
				((idBuh > 0) ? "AND " + idBuhField + " = @IdBuh " : "") +
				"AND " + typeBaseField + " = 1) " +
				" OR " +
				"EXISTS (SELECT *" +
				" FROM " + buhParamContrTableName +
				" WITH (NOLOCK) WHERE " + idDocField + " = @IdDoc " +
				((idBuh > 0) ? "AND " + idBuhField + " = @IdBuh " : "") +
				"AND " + typeBaseField + " = 1) THEN '�' ELSE '' END " + typeBaseField,
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@IdDoc", SqlDbType.Int, idDoc);
					if (idBuh > 0)
						AddParam(cmd, "@IdBuh", SqlDbType.Int, idBuh);
				}, null);
		}

		public List<int> GetSentDocToIc(int idDoc, int typeBase)
		{
			return GetRecords<int>(
				"SELECT " + idBuhField +
				" FROM " + tableName +
				" WITH (NOLOCK) WHERE " + idDocField + " = @IdDoc AND " + typeBaseField + " = @TypeBase " +
				" UNION " +
				"SELECT " + idBuhField +
				" FROM " + buhParamContrTableName +
				" WITH (NOLOCK) WHERE " + idDocField + " = @IdDoc AND " + typeBaseField + " = @TypeBase ",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@IdDoc", SqlDbType.Int, idDoc);
					AddParam(cmd, "@TypeBase", SqlDbType.Int, typeBase);
				});
		}

		#endregion
	}
}
