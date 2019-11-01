namespace Kesco.Lib.Win.Data.DALC.Directory
{
	/// <summary>
	/// Summary description for UnitDimensionDALC.
	/// </summary>
	public class UnitDimensionDALC : DALC
	{
		public UnitDimensionDALC(string connectionString) : base(connectionString)
		{
			tableName = "Справочники.dbo.ЕдиницыИзмерения";

			idField = "КодЕдиницыИзмерения";
			nameField = "ЕдиницаРус";
		}
	}
}
