using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.NoSign
{
	[Option("Image.NoSing.�������������", typeof(�������������))]
	[SeparateOption("Image.NoSing.�������������", typeof(�����������))]
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
	AND TI.����������=2)\n" + (IsSeparate()?"": @" OR EXISTS(SELECT * FROM ���������.dbo.vw��������������� TD  WITH(NOLOCK) WHERE TD.������������=T0.������������)
	AND NOT EXISTS(
	SELECT * FROM ���������.dbo.����������������� TP WITH(NOLOCK)
	WHERE TP.������������=T0.������������ AND TP.����������=1 AND TP.����������������������� IS NULL
				))");
		}
	}
}