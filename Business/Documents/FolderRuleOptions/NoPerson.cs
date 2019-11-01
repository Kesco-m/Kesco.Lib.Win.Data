using System;
using Kesco.Lib.Win.Data.Business.Persons;

namespace Kesco.Lib.Win.Data.Business.Documents.FolderRuleOptions
{
	public class NoPerson : PersonOption
	{
		public override string GetCaption() { return StringResources.P1; }
		public override string GetHtml() { return StringResources.P1 + ": [<A href=#" + Name + ">" + (Person == null ? StringResources.Select : PersonName) + "</A>]"; }
		public override string GetShortText() { return StringResources.P2 + ": " + PersonName; }
		public override string GetText() { return StringResources.P1 + ": [" + PersonName + "]"; }
		public override string GetSQL() { return null; }
		public override bool Validate(bool throwOnError)
		{
			if (Person == null)
			{
				if (throwOnError) throw new Exception(StringResources.P3);
				return false;
			}
			return true;
		}
		public NoPerson(string name) : base(name) { }
	}
}