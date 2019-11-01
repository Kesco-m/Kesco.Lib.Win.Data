using System.Resources;
using System.Text.RegularExpressions;
using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns
{
    /// <summary>
    /// Summary description for TextOption.
    /// </summary>
	public class TextOption : ValueOption
	{
		public enum Modes
		{
			Contains,
			BeginsWith
		}

		public TextOption(XmlElement el) : base(el)
		{
		}

		public virtual Modes Mode
		{
			get { return el.GetAttribute("mode").ToLower().Equals("intext") ? Modes.Contains : Modes.BeginsWith; }
		}

		public string GetSQLCondition2(string field)
		{
			return GetSQLCondition(field);
		}

		protected override string GetSQLCondition(string field)
		{
			string val = Data.DALC.Documents.Search.Options.PrepareTextParameter(Value).Trim();
			switch(Mode)
			{
				case Modes.Contains:
					return field + " LIKE '%" + Regex.Replace(val, "[ ]{1,}", "%") + "%'";
				default:
					return "' '+" + field + " LIKE '" + Regex.Replace(" " + val, "[ ]{1,}", "% ") + "%'";
			}
		}

		public override string GetHTML()
		{
			var resources = new ResourceManager(typeof(TextOption));
			string s0 = (Mode == Modes.Contains ? resources.GetString("Containing") : resources.GetString("StartingWith"));
			string s1 = Value.Length == 0 ? emptyValueText : GetItemText(Value);

			return htmlPrefix +
				   s0 +
				   htmlLinkPrefix +
				   htmlItemPrefix +
				   s1 +
				   htmlItemPostfix +
				   htmlLinkPostfix +
				   htmlPostfix;
		}

		public override string GetText()
		{
			if(Value.Length == 0)
				return "";
			var resources = new ResourceManager(typeof(TextOption));

			string s0 = (Mode == Modes.Contains
							 ? resources.GetString("Containing")
							 : resources.GetString("StartingWith"));
			string s1 = GetItemText(Value);

			return htmlPrefix + s0 +
				   textItemPrefix + s1 + textItemPostfix +
				   htmlPostfix;
		}

		public override string GetShortText()
		{
			if(Value.Length == 0)
				return "";
			var resources = new ResourceManager(typeof(TextOption));

			string s0 = (Mode == Modes.Contains
							 ? resources.GetString("Containing")
							 : resources.GetString("StartingWith"));
			string s1 = GetItemText(Value);

			return shortTextPrefix + s0 +
				   textItemPrefix + s1 + textItemPostfix +
				   shortTextPostfix;
		}
	}
}