using System.Data;

namespace Kesco.Lib.Win.Data.Business.Persons
{
	public class Person : BaseEE
	{
		string shortName;

		#region ACCESSORS

		/// <summary>
		/// кличка
		/// </summary>
		public string ShortName
		{
			get
			{
				LoadIfDelayed();
				return shortName;
			}
		}

		#endregion

		#region Fields

		protected override string FillFrom_Table
		{
			get { return "Справочники.dbo.vwЛица"; }
		}
		protected override string ID_Field
		{
			get { return "КодЛица"; }
		}

		const string shortName_field = "Кличка";

		#endregion

		#region Constructors

		public Person(int id) : base(id)	//existing
		{
			connectionString = Settings.DS_person;
		}

		#endregion

		protected override void Fill(DataRow row)
		{
			base.Fill(row);
			shortName = (string)row[shortName_field];
		}
	}
}
