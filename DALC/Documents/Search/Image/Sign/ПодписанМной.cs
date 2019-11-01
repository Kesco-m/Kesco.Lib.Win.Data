using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.Sign
{
    /// <summary>
    /// Summary description for �������������.
    /// </summary>
    /// 
    [Option("Image.Sing.������������", typeof (������������))]
    public class ������������ : MyOption
    {
        protected ������������(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"Image.NoSing.��������������"};
            NegativeValueOption = new[] {"Image.NoSing.����������"};
            shortTextPrefix = Resources.GetString("shortTextPrefix");
        }

        public override string GetSQL(bool throwOnError)
        {
            return
				@"EXISTS (SELECT *
                    FROM ���������.dbo.����������������� TI WITH(NOLOCK)
                    WHERE TI.������������=T0.������������ AND TI.����������<>101 AND TI.����������������������� IS NOT NULL AND
                    (TI.��������������� = " +
                Value + " OR TI.������������� = " + Value + "))" + Environment.NewLine;
        }
    }
}