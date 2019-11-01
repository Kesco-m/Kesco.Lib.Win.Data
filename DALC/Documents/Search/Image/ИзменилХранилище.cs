using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image
{
	[Option("Image.����������������", typeof(����������������), typeof(���������), 1)]
	public class ���������������� : EmployeeListOption
	{
		protected ����������������(XmlElement el) : base(el)
		{
			Mode = Modes.OR;
			emptyValueText = Resources.GetString("emptyValueText");

			shortTextPrefix = Resources.GetString("shortTextPrefix");
			shortTextPostfix = "";

			htmlPrefix = Resources.GetString("htmlPrefix");
			htmlPrefix2 = Resources.GetString("htmlPrefix2");
			htmlPostfix = "";

			textItemPrefix = "[";
			textItemPostfix = "]";
		}

		public override string GetSQL(bool throwOnError)
		{
			return null;
		}
	}
}