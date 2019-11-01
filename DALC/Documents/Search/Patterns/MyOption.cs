using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns
{
	public class MyOption:Option
	{
		public MyOption(XmlElement el):base(el)
		{
		}

		public string Value
		{
			get
			{
				return el.GetAttribute("value");
			}
		}
		public virtual void SetValue(string newValue)
		{
		    string s = newValue.Trim();
		    if (s.Length == 0) el.RemoveAttribute("value");
		    el.SetAttribute("value", s);
		}
	}
}
