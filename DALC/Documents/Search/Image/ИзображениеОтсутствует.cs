using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image
{
    [Option("����������������������", typeof (����������������������))]
    public class ���������������������� : Option
    {
        protected ����������������������(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"Image.�����������", "�������������"};
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPostfix = "";
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"
                NOT EXISTS (SELECT *
                FROM ���������.dbo.vw��������������������� TI WITH(NOLOCK)
                WHERE TI.������������=T0.������������)
                ";
        }
    }
}