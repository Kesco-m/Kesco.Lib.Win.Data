using System;
using Kesco.Lib.Win.Data.Business.Corporate;

namespace Kesco.Lib.Win.Data.Business.Documents.FolderRuleOptions
{
	public class NotSignetBy : EmployeeOption
	{
		public override string GetCaption() { return StringResources.E6; }
		public override string GetHtml() { return StringResources.E6 + ": [<A href=#" + Name + ">" + (Employee == null ? StringResources.Select : EmployeeName) + "</A>]"; }
		public override string GetShortText() { return StringResources.E7 + ": " + EmployeeName; }
		public override string GetText() { return StringResources.E6 + ": [" + EmployeeName + "]"; }
		public override string GetSQL() { return null; }
		public override bool Validate(bool throwOnError)
		{
			if (Employee == null)
			{
				if (throwOnError) throw new Exception(StringResources.E8);
				return false;
			}
			return true;
		}
		public NotSignetBy(string name) : base(name) { }
	}
}