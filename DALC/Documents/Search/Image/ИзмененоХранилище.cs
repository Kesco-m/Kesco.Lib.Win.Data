using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image
{
	/// <summary>
	/// Summary description for ��������������.
	/// </summary>
	[Option("Image.�����������������", typeof(�����������������), typeof(���������),2)]
	public class ����������������� : DateOption
	{
		protected �����������������(XmlElement el) : base(el)
		{
			emptyValueText = Resources.GetString("emptyValueText");
			shortTextPrefix = htmlPrefix = Resources.GetString("shortTextPrefix");
			shortTextPostfix = htmlPostfix = "";
		}

		public override string GetSQL(bool throwOnError)
		{
			return null;
		}
	}
}