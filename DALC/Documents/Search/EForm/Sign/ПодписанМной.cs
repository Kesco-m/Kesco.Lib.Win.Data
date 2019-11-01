using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm.Sign
{
    [Option("EForm.Sing.������������", typeof (������������))]
    public class ������������ : MyOption
    {
        protected ������������(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"EForm.NoSing.��������������"};
            NegativeValueOption = new[] {"EForm.NoSing.����������"};

            shortTextPrefix = Resources.GetString("shortTextPrefix");
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"
                EXISTS (SELECT *
                FROM ���������.dbo.����������������� TI WITH(NOLOCK)
                WHERE TI.������������=T0.������������ AND TI.����������������������� IS NULL AND 
                (TI.��������������� = " +
                Value + " OR TI.������������� = " + Value + "))" + Environment.NewLine;
        }
    }
}