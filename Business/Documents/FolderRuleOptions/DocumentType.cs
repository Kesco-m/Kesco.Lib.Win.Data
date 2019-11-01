using System;

namespace Kesco.Lib.Win.Data.Business.Documents.FolderRuleOptions
{
	public class DocumentType : DocumentTypeOption
	{
		public override string GetCaption() { return StringResources.DT2; }
		public override string GetHtml()
		{
			return StringResources.DT1 + " [<A href=#" + Name + ">" + (Type == null ? StringResources.Select : (TypeName +
				(((Filter > 1) ? StringResources.DT5 : "") + (((Filter & 1) > 0) ? StringResources.DT6 : "")))) + "</A>]";
		}
		public override string GetShortText() { return StringResources.DT4 + ": " + TypeName + (((Filter > 1) ? StringResources.DT5 : "") + (((Filter & 1) > 0) ? StringResources.DT6 : "")); }
		public override string GetText() { return StringResources.DT1 + " [" + TypeName + "]" + (((Filter > 1) ? StringResources.DT5 : "") + (((Filter & 1) > 0) ? StringResources.DT6 : "")); }
		public override string GetSQL() { return null; }
		public override bool Validate(bool throwOnError)
		{
			if (Type == null)
			{
				if (throwOnError) throw new Exception(StringResources.DT3);
				return false;
			}
			return true;
		}
		public DocumentType(string name) : base(name) { }
	}
}