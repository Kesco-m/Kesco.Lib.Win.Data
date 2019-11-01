using System;
using Kesco.Lib.Win.Data.Options;

namespace Kesco.Lib.Win.Data.Business.Documents.FolderRuleOptions
{
	public class MessageText : SimpleTextOption
	{
		public override string GetCaption() { return StringResources.MT1; }
		public override string GetHtml() { return StringResources.MT1 + ": [<A href=#" + Name + ">" + (Text.Equals("") ? StringResources.Select : ("'" + Text + "'")) + "</A>]"; }
		public override string GetShortText() { return StringResources.MT2 + ": '" + Text + "'"; }
		public override string GetText() { return 1 + ": '" + Text + "'"; }
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
		public MessageText(string name) : base(name) { }
	}
}