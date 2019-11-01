using System;
using Kesco.Lib.Win.Data.Business.Corporate;

namespace Kesco.Lib.Win.Data.Business.Documents.FolderRuleOptions
{
	public class SignedBy : EmployeeOption
	{
		public override string GetCaption() { return StringResources.E9; }
		public override string GetHtml() { return StringResources.E9 + ": [<A href=#" + Name + ">" + (Employee == null ? StringResources.Select : EmployeeName) + "</A>]"; }
		public override string GetShortText() { return StringResources.Sign + ": " + EmployeeName; }
		public override string GetText() { return StringResources.E9 + ": [" + EmployeeName + "]"; }
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
		public SignedBy(string name) : base(name) { }
	}
}