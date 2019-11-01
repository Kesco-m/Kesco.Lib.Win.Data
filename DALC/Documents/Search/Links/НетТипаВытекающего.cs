using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Document;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Links
{
    /// <summary>
    /// Summary description for НетТипаВытекающего.
    /// </summary>

    [Option("НетТипаВытекающего", typeof (НетТипаВытекающего))]
    public class НетТипаВытекающего : ТипДокумента
    {
        protected НетТипаВытекающего(XmlElement el) : base(el)
        {
            emptyValueText = Resources.GetString("emptyValueText");

            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPrefix2 = Resources.GetString("htmlPrefix2");
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
                      vwДокументы TL WITH(NOLOCK) ON TI.КодДокументаВытекающего = TL.КодДокумента
					WHERE " +
                ((val.Length == 0) ? "" : "(TL.КодТипаДокумента IN (" + val + ")) AND ") +
                "(TI.КодДокументаОснования=T0.КодДокумента))";
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
