using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.Send
{
	/// <summary>
	/// Summary description for Отправитель.
	/// </summary>
	[Option("Image.Отправитель", typeof(Отправитель))]
	public class Отправитель : EmployeeListOption
	{
		protected Отправитель(XmlElement el) : base(el)
		{
			emptyValueText = Resources.GetString("emptyValueText");
			shortTextPrefix = Resources.GetString("shortTextPrefix");
			shortTextPostfix = "";

			htmlPrefix = Resources.GetString("htmlPrefix");
			htmlPrefix2 = Resources.GetString("htmlPrefix2");
			htmlPostfix = "";

			textItemPrefix = "[";
			textItemPostfix = "]";
		}

		public override bool OpenWindow()
		{
			return false;
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