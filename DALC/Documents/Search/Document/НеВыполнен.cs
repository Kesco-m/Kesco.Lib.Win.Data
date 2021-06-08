using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Document
{
	[Option("����������", typeof(����������))]
	public class ���������� : Option
	{
		protected ����������(XmlElement el)
			: base(el)
		{
			NegativeOption = new[] { "��������"};
			shortTextPrefix = Resources.GetString("shortTextPrefix");
		}

		public override string GetSQL(bool throwOnError)
		{
			return
				@"(EXISTS(SELECT * FROM ���������.dbo.vw��������������� TD  WITH(NOLOCK) WHERE TD.������������=T0.������������)
				AND NOT EXISTS (
					SELECT * FROM ���������.dbo.����������������� TP WITH(NOLOCK)
					WHERE TP.������������=T0.������������ AND TP.����������=1 AND TP.����������������������� IS NULL
				))
			OR (NOT EXISTS (SELECT * FROM ���������.dbo.vw��������������� TD1  WITH(NOLOCK) WHERE TD1.������������=T0.������������)
				AND EXISTS (SELECT * FROM ���������.dbo.vw��������������������� TI  WITH(NOLOCK) WHERE TI.������������=T0.������������
					AND NOT EXISTS (
						SELECT * FROM ���������.dbo.����������������� TP WITH(NOLOCK)
						WHERE  TP.����������=2 AND TP.����������������������� = TI.�����������������������)))
";
		}
	}
}