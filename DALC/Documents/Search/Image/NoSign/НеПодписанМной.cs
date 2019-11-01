using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.NoSign
{
    [Option("Image.NoSing.��������������", typeof (��������������))]
    public class �������������� : MyOption
    {
        protected ��������������(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"Image.Sing.������������"};
            NegativeValueOption = new[] {"Image.Sing.��������"};
            shortTextPrefix = Resources.GetString("shortTextPrefix");
        }

        public override string GetSQL(bool throwOnError)
        {
            return
				@"
                NOT EXISTS (SELECT *
                FROM ���������.dbo.����������������� TI WITH(NOLOCK)
                WHERE TI.������������=T0.������������ AND TI.����������������������� IS NOT NULL AND TI.����������<>101 AND
                (TI.��������������� = " +
                Value + " OR TI.������������� = " + Value + "))" + Environment.NewLine;
        }
    }
}