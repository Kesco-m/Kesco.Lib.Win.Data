using System;
using Kesco.Lib.Win.Data.Business.Persons;

namespace Kesco.Lib.Win.Data.Business.Documents.FolderRuleOptions
{
	public class Person : PersonOption
	{
		public override string GetCaption() { return StringResources.P4; }
		public override string GetHtml() { return StringResources.P4 + ": [<A href=#" + Name + ">" + (Person == null ? StringResources.Select : PersonName) + "</A>]"; }
		public override string GetShortText() { return StringResources.Person + ": " + PersonName; }
		public override string GetText() { return StringResources.P4 + ": [" + PersonName + "]"; }
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
		public Person(string name) : base(name) { }
	}
}