using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm
{
    [Option("������������������", typeof (������������������))]
    public class ������������������ : Option
    {
        protected ������������������(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"�������", "�������������"};
            shortTextPrefix = Resources.GetString("shortTextPrefix");
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"
                NOT EXISTS (SELECT *
                FROM ���������.dbo.vw��������������� TI WITH(NOLOCK)
                WHERE TI.������������=T0.������������)
                ";
        }
    }
}