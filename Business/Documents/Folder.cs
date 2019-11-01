using System.Data;

namespace Kesco.Lib.Win.Data.Business.Documents
{
	public class Folder:BaseEE
	{
		string name;

		#region ACCESSORS

		public string Name
		{
			get
			{
				LoadIfDelayed();
				return name;
			}
			set
			{
				name=value;
			}
		}

		#endregion

		#region Constructor
		public Folder(int id):base(id)
		{
			
		}
		#endregion

		#region DB

		protected override string FillFrom_Table{	get{return "���������.dbo.vw���������������";}}
		protected override string ID_Field		{	get{return "������������������";}}
		protected string nameField = "���������������";

		protected override void Fill(DataRow row)
		{
			base.Fill (row);
			name=row.IsNull(nameField)?"":(string)row[nameField];
		}
		
		#endregion
	}
}
