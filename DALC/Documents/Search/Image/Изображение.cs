using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image
{
    [Option("Image.�����������", typeof (�����������))]
    public class ����������� : Option
    {
        protected �����������(XmlElement el)  : base(el)
        {
            NegativeOption = new[] {"����������������������", "������������������������"};
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPostfix = "";
        }


        public override string GetSQL(bool throwOnError)
        {
            return null;
        }
    }
}