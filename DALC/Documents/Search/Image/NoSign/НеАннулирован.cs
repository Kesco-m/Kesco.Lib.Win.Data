using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.NoSign
{
	[Option("Image.NoSing.�������������", typeof(�������������))]
	public class ������������� : Option
	{

		protected �������������(XmlElement el) : base(el)
		{
			NegativeOption = new[ ] { "Image.Sing.������������" };
			shortTextPrefix = Resources.GetString("shortTextPrefix");
		}

		public override string GetSQL(bool throwOnError)
		{
			return @"NOT EXISTS (SELECT *
                FROM ���������.dbo.����������������� TI WITH(NOLOCK)
                WHERE TI.������������=T0.������������
                AND TI.����������=2)
                ";
		}
	}
}