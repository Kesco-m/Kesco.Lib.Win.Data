using System;

namespace Kesco.Lib.Win.Data.Business.Documents.FolderRuleOptions
{
	public class Document : DocumentOption
	{
		public override string GetCaption() { return StringResources.D1; }
		public override string GetHtml() { return StringResources.D1 + ": [<A href=#" + Name + ">" + (Document == null ? StringResources.Select : DocumentName) + "</A>]"; }
		public override string GetShortText() { return StringResources.D2 + ": " + DocumentName; }
		public override string GetText() { return StringResources.D1 + ": [" + DocumentName + "]"; }
		public override string GetSQL() { return null; }
		public override bool Validate(bool throwOnError)
		{
			if (Document == null)
			{
				if (throwOnError) throw new Exception(StringResources.D3);
				return false;
			}
			return true;
		}
		public Document(string name) : base(name) { }
	}
}