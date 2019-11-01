using System;
using Kesco.Lib.Win.Data.Business.Corporate;

namespace Kesco.Lib.Win.Data.Business.Documents.FolderRuleOptions
{
	public class MessageFrom : EmployeeOption
	{
		public override string GetCaption() { return StringResources.E1; }
		public override string GetHtml() { return StringResources.E2 + ": [<A href=#" + Name + ">" + (Employee == null ? StringResources.Select : EmployeeName) + "</A>]"; }
		public override string GetShortText() { return StringResources.From + ": " + EmployeeName; }
		public override string GetText() { return StringResources.E2 + ": [" + EmployeeName + "]"; }
		public override string GetSQL() { return null; }
		public override bool Validate(bool throwOnError)
		{
			if (Employee == null)
			{
				if (throwOnError) throw new Exception(StringResources.E3);
				return false;
			}
			return true;
		}

		public MessageFrom(string name) : base(name) { }
	}
}