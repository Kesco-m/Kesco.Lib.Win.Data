using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Document;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Links
{
    /// <summary>
    /// Summary description for ТипОснования.
    /// </summary>
    [Option("ТипОснования", typeof (ТипОснования))]
    public class ТипОснования : ТипДокумента
    {
        protected ТипОснования(XmlElement el) : base(el)
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
                      vwДокументы TL WITH(NOLOCK) ON TI.КодДокументаОснования = TL.КодДокумента
					WHERE " +
                ((val.Length == 0) ? "" : "(TL.КодТипаДокумента IN (" + val + ")) AND ") +
                "(TI.КодДокументаВытекающего=T0.КодДокумента))";
        }

        public override string GetText()
        {
            return string.IsNullOrEmpty(NormalizeC(Value))
                ? htmlPrefix + emptyValueText + htmlPostfix
                : base.GetText();
        }

        public override string GetShortText()
        {
            return string.IsNullOrEmpty(NormalizeC(Value))
                       ? shortTextPrefix + emptyValueText + shortTextPostfix
                       : base.GetText();
        }
    }
}
