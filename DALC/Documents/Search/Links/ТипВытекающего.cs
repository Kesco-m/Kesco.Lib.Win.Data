using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Document;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Links
{
    /// <summary>
    /// Summary description for ТипВытекающего.
    /// </summary>
    [Option("ТипВытекающего", typeof (ТипВытекающего))]
    public class ТипВытекающего : ТипДокумента
    {
        protected ТипВытекающего(XmlElement el) : base(el)
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
                @"EXISTS (SELECT TI.*
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
