using System.Text.RegularExpressions;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.Send
{
	/// <summary>
	/// Summary description for ТипФиноперации.
	/// </summary>
	[Option("Image.ТипОтправки", typeof(ТипОтправки), typeof(Отправитель),1)]
	public class ТипОтправки : ValueOption
	{
		protected ТипОтправки(XmlElement el) : base(el)
		{
			emptyValueText = Resources.GetString("emptyValueText");

			shortTextPrefix = Resources.GetString("shortTextPrefix");
			shortTextPostfix = "";

			htmlPrefix = Resources.GetString("htmlPrefix");
			htmlPostfix = "";

			textItemPrefix = "[";
			textItemPostfix = "]";
		}

		public override string GetItemText(string key)
		{
			string str = emptyValueText;
			if(!Regex.IsMatch(key, "^\\d{0,9}$"))
				return emptyValueText;
			int val = int.Parse(key);
			switch(val)
			{
				case 2:
					str = Resources.GetString("Email");
					break;
				case 1:
					str = Resources.GetString("Fax");
					break;
			}

			return str;
		}

		public override string GetSQL(bool throwOnError)
		{
			return null;
		}

		public override string GetText()
		{
			return string.Empty;
		}

		public override string GetShortText()
		{
			return string.Empty;
		}
	}
}