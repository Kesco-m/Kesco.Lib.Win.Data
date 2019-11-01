using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
	/// <summary>
	/// DAL-��������� ��� ������� � ������� ���������.dbo.���������
	/// </summary>
	public class FieldTypeDALC : DALC
	{
		private const string dataTypesField = "����������";
		private const string fieldNameField = "�������";
		private const string digitCountNeededField = "��������������������������";
		private const string urlNeededField = "�����URL������";
		private const string dataSourceNeededField = "�����SQL������";
		private const string substFormTitleNeededField = "�������������������������";
		private const string serviceUrlField = "������";
		private const string detailsUrlField = "��������";

		public FieldTypeDALC(string connectionString) : base(connectionString)
		{
			tableName = "���������.dbo.���������";

			idField = "�����������";
			nameField = "�������";
		}

		#region Accessors

		public string DataTypesField
		{
			get { return dataTypesField; }
		}

		public string DigitCountNeededField
		{
			get { return digitCountNeededField; }
		}

		public string URLNeededField
		{
			get { return urlNeededField; }
		}

		public string DataSourceNeededField
		{
			get { return dataSourceNeededField; }
		}

		public string SubstFormTitleNeededField
		{
			get { return substFormTitleNeededField; }
		}

		public string ServiceUrlField
		{
			get { return serviceUrlField; }
		}

		public string DetailsUrlField
		{
			get { return detailsUrlField; }
		}

		public string FieldNameField
		{
			get { return fieldNameField; }
		}

		#endregion

		#region Get Data

		public DataSet GetFieldTypes(string dataType, string name)
		{
			DataSet ds = GetFieldTypesByName(name);
			return ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0
					   ? ds
					   : GetFieldTypesByDataType(dataType);
		}

		public DataSet GetFieldTypesByDataType(string dataType)
		{
			return GetData("SELECT " +
					idField + ", " +
					nameField + ", " +
					fieldNameField + ", " +
					digitCountNeededField + ", " +
					urlNeededField + ", " +
					dataSourceNeededField + ", " +
					substFormTitleNeededField +
					" FROM " + tableName +
					" WHERE " +
					dataTypesField + " LIKE @TypeLikeString",
					delegate(SqlCommand cmd)
					{
						AddParam(cmd, "@TypeLikeString", SqlDbType.NVarChar, "%/" + dataType + "/%");
					});
		}

		public DataSet GetFieldTypesByName(string name)
		{
			return GetData("SELECT " + idField + ", " + nameField + ", " + fieldNameField + ", " + digitCountNeededField + ", "
				+ urlNeededField + ", " + dataSourceNeededField + ", " + substFormTitleNeededField + " FROM " + tableName +
				" WHERE " + fieldNameField + " = @Name",
					delegate(SqlCommand cmd)
					{
						name = Regex.Replace(name, @"\d", "");
						AddParam(cmd, "@Name", SqlDbType.NVarChar, name);
					});
		}

		#endregion
	}
}