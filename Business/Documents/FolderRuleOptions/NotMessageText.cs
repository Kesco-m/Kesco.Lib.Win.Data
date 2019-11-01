using System;
using Kesco.Lib.Win.Data.Options;

namespace Kesco.Lib.Win.Data.Business.Documents.FolderRuleOptions
{
	public class NotMessageText : SimpleTextOption
	{
		public override string GetCaption() { return StringResources.MT4; }
		public override string GetHtml() { return StringResources.MT4 + ": [<A href=#" + Name + ">" + (Text.Equals("") ? StringResources.Select : ("'" + Text + "'")) + "</A>]"; }
		public override string GetShortText() { return StringResources.MT5 + ": '" + Text + "'"; }
		public override string GetText() { return StringResources.MT4 + ": '" + Text + "'"; }
		public override string GetSQL() { return null; }
		public override bool Validate(bool throwOnError)
		{
			if (Text.Equals(""))
			{
				if (throwOnError) throw new Exception(StringResources.MT3);
				return false;
			}
			return true;
		}
		public NotMessageText(string name) : base(name) { }
	}
}