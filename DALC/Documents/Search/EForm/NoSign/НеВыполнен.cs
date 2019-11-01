using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm.NoSign
{
    [Option("EForm.NoSing.����������", typeof (����������))]
    public class ���������� : Option
    {
        protected ����������(XmlElement el)
            : base(el)
        {
            NegativeValueOption = new[] {"EForm.Sing.��������"};
            shortTextPrefix = Resources.GetString("shortTextPrefix");
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"
                NOT EXISTS (SELECT *
                FROM ���������.dbo.����������������� TI WITH(NOLOCK)
                WHERE TI.������������=T0.������������
                AND TI.����������=1 AND TI.����������������������� IS NULL)
                ";
        }
    }
}