using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
	/// <summary>
	/// DAL-компонент для доступа к таблице Документы.dbo.ТипыПолей
	/// </summary>
	public class FieldTypeDALC : DALC
	{
		private const string dataTypesField = "ТипыДанных";
		private const string fieldNameField = "ИмяПоля";
		private const string digitCountNeededField = "НужноЧислоДесятичныхЗнаков";
		private const string urlNeededField = "НуженURLПоиска";
		private const string dataSourceNeededField = "НуженSQLЗапрос";
		private const string substFormTitleNeededField = "НуженЗаголовокФормыПоиска";
		private const string serviceUrlField = "Сервис";
		private const string detailsUrlField = "Сущность";

		public FieldTypeDALC(string connectionString) : base(connectionString)
		{
			tableName = "Документы.dbo.ТипыПолей";

			idField = "КодТипаПоля";
			nameField = "ТипПоля";
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