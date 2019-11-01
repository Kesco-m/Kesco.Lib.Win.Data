using System;
using Kesco.Lib.Win.Data.Business.Corporate;

namespace Kesco.Lib.Win.Data.Business.Documents.FolderRuleOptions
{
	public class MessageTo : EmployeeOption
	{
		public override string GetCaption() { return StringResources.E4; }
		public override string GetHtml() { return StringResources.E4 + ": [<A href=#" + Name + ">" + (Employee == null ? StringResources.Select : EmployeeName) + "</A>]"; }
		public override string GetShortText() { return StringResources.To + ": " + EmployeeName; }
		public override string GetText() { return StringResources.E4 + ": [" + EmployeeName + "]"; }
		public override string GetSQL() { return null; }
		public override bool Validate(bool throwOnError)
		{
			if (Employee == null)
			{
				if (throwOnError) throw new Exception(StringResources.E5);
				return false;
			}
			return true;
		}
		public MessageTo(string name) : base(name) { }
	}
}