using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm.NoSign
{
	[Option("EForm.NoSing.��������������", typeof(��������������))]
	[SeparateOption("EForm.NoSing.��������������", typeof(�������))]
	public class �������������� : MyOption
	{
		protected ��������������(XmlElement el) : base(el)
		{
			NegativeOption = new[] { "EForm.Sing.������������" };
			NegativeValueOption = new[] { "EForm.Sing.��������", "EForm.Sing.��������"};
			shortTextPrefix = Resources.GetString("shortTextPrefix");
		}

		public override string GetSQL(bool throwOnError)
		{
			return
				@"
                    NOT EXISTS (SELECT *
                    FROM ���������.dbo.����������������� TI WITH(NOLOCK)
                    WHERE TI.������������=T0.������������ AND TI.����������������������� IS NULL AND 
                    (TI.��������������� = " +
				Value + " OR TI.������������� = " + Value + "))" + Environment.NewLine;
		}
	}
}