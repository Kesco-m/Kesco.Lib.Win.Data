using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Document;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Links
{
    /// <summary>
    /// Summary description for НетТипаОснования.
    /// </summary>

    [Option("НетТипаОснования", typeof (НетТипаОснования))]
    public class НетТипаОснования : ТипДокумента
    {
        protected НетТипаОснования(XmlElement el) : base(el)
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
					FROM vwСвязиДокументов TI WITH(NOLOCK) INNER JOIN
                      vwДокументы TL WITH(NOLOCK) ON TI.КодДокументаОснования = TL.КодДокумента
					WHERE " +
                ((val.Length == 0) ? "" : "(TL.КодТипаДокумента IN (" + val + ")) AND ") +
                "(TI.КодДокументаВытекающего=T0.КодДокумента))";
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
