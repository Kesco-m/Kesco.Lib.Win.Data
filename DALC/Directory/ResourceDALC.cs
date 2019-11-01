using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Directory
{
    /// <summary>
    /// DAL-компонент для доступа к таблице Справочники.dbo.Ресурсы
    /// </summary>
	public class ResourceDALC : DALC
	{
		private string unitRusField;
		private string parentField;
		private string precisionField;
        private const string unitDimensionIDField = "КодЕдиницыИзмерения";

		public ResourceDALC(string connectionString)
			: base(connectionString)
		{
			tableName = "Справочники.dbo.Ресурсы";

			idField = "КодРесурса";
			nameField = "РесурсРус";

			unitRusField = "РесурсРус";
			precisionField = "Точность";

			parentField = "Parent";
		}

		#region Accessors

		public string UnitRusField
		{
			get { return unitRusField; }
		}

		public string ParentField
		{
			get { return parentField; }
		}

		public string UnitDimensionIDField
		{
			get { return unitDimensionIDField; }
		}

		public string PrecisionField
		{
			get { return precisionField; }
		}

		#endregion

		#region Get Data

		public bool IsMoney(int id)
		{
			return FieldExists(
				" WHERE " + idField + " = @ID " +
				" AND " + parentField + " = 1",
				delegate(SqlCommand cmd)
				{
					AddParam(cmd, "@ID", SqlDbType.Int, id);
				});
		}

		#endregion
	}
}