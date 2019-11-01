using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Document
{
    [Option("�������", typeof (�������))]
    public class ������� : MyOption
    {
        protected �������(XmlElement el) : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPostfix = "";
        }

        public override string GetSQL(bool throwOnError)
        {
            return Value.Length > 0
                       ? @"
EXISTS (SELECT *
FROM ���������.dbo.vw���������������� TI WITH(NOLOCK)
WHERE TI.������������=T0.������������ AND �������������=" +
                         Value + @")
"
                       : @"
EXISTS (SELECT *
FROM ���������.dbo.vw���������������� TI WITH(NOLOCK)
WHERE TI.������������=T0.������������ AND �������������=@EmpID)
";
        }
    }
}
