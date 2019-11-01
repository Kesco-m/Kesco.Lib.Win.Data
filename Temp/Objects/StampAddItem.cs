using System;
using System.Data;
using Kesco.Lib.Win.Data.DALC.Documents;

namespace Kesco.Lib.Win.Data.Temp.Objects
{
	public class StampAddItem : IDObject
	{
		private readonly string name;
		private readonly bool canInsert;
		private readonly string needSigns;

		public StampAddItem(IDataRecord dr, StampDALC data) : base ( (int)dr[data.IDField])
		{
			name = (string)dr[data.NameField];
			canInsert = Convert.ToBoolean( dr[data.CanInsertField]);
			try
			{
				needSigns = (string)dr[data.EmployeesField];
			}
			catch { }
		}

		public string Name
		{
			get { return name; }
		}

		public bool CanInsert
		{
			get { return canInsert; }
		}

		public string NeedSigns
		{
			get { return needSigns; }
		}
	}
}
