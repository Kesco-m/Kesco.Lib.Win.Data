using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Document;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Links
{
    /// <summary>
    /// Summary description for ����������������.
    /// </summary>

    [Option("����������������", typeof (����������������))]
    public class ���������������� : ������������
    {
        protected ����������������(XmlElement el) : base(el)
        {
            emptyValueText = Resources.GetString("emptyValueText");

            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPrefix2 = Resources.GetString("htmlPrefix");
            htmlPostfix = "";

            textItemPrefix = "[";
            textItemPostfix = "]";
        }

        public override bool OpenWindow()
        {
            return false;
        }

        public override string GetSQL(bool throwOnError)
        {
            string val = NormalizeC(Value);

            return
                @"NOT EXISTS (SELECT TI.*
					FROM vw��������������� TI WITH(NOLOCK) INNER JOIN
                      vw��������� TL WITH(NOLOCK) ON TI.��������������������� = TL.������������
					WHERE " +
                ((val.Length == 0) ? "" : "(TL.���������������� IN (" + val + ")) AND ") +
                "(TI.�����������������������=T0.������������))";
        }

        public override string GetText()
        {
            string val = NormalizeC(Value);
            return val.Length == 0 ? htmlPrefix + emptyValueText + htmlPostfix : base.GetText();
        }

        public override string GetShortText()
        {
            string val = NormalizeC(Value);
            return val.Length == 0 ? shortTextPrefix + emptyValueText + shortTextPostfix : base.GetText();
        }
    }
}
