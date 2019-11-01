using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Document
{
    [Option("ВРаботе", typeof (ВРаботе))]
    public class ВРаботе : MyOption
    {
        protected ВРаботе(XmlElement el) : base(el)
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
FROM Документы.dbo.vwДокументыВРаботе TI WITH(NOLOCK)
WHERE TI.КодДокумента=T0.КодДокумента AND КодСотрудника=" +
                         Value + @")
"
                       : @"
EXISTS (SELECT *
FROM Документы.dbo.vwДокументыВРаботе TI WITH(NOLOCK)
WHERE TI.КодДокумента=T0.КодДокумента AND КодСотрудника=@EmpID)
";
        }
    }
}
