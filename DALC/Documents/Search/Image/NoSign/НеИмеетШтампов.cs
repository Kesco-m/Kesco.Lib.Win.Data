using System;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.NoSign
{
    /// <summary>
    /// Summary description for НеИмеетШтампов.
    /// </summary>
    [Option("Image.NoSing.НеИмеетШтампов", typeof (НеИмеетШтампов))]
    public class НеИмеетШтампов : ListOption
    {

        protected НеИмеетШтампов(XmlElement el) : base(el)
        {
            NegativeValueOption = new[] {"Image.Sing.ИмеетШтампы", "Image.Sing.Подписан"};

            emptyValueText = Resources.GetString("emptyValueText");

            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPrefix2 = Resources.GetString("htmlPrefix2");
            htmlPostfix = "";

            textItemPrefix = "[";
            textItemPostfix = "]";
        }

        protected override string GetSQLCondition(string pattern)
        {
            string[] values = GetValues(false);
            return values.Aggregate("", (current, t) => current + ((current.Length > 0 ? (Mode == Modes.AND ? " OR " : " AND ") : "") + "(" + pattern.Replace("@VAL", GetSQLParameter(t)) + ")"));
        }

        public override string GetSQL(bool throwOnError)
        {
            try
            {
                string[] vals = GetValues(throwOnError);

                return vals.Length != 0
                           ? GetSQLCondition(
                               @"
NOT EXISTS (SELECT *
FROM Документы.dbo.ПодписиДокументов TI WITH(NOLOCK)
WHERE TI.КодДокумента=T0.КодДокумента AND TI.ТипПодписи = 100 AND
(TI.КодШтампа = @VAL))"
                                 )
                           : @"
NOT EXISTS (SELECT *
FROM Документы.dbo.ПодписиДокументов TI WITH(NOLOCK)
WHERE TI.КодДокумента=T0.КодДокумента AND TI.ТипПодписи = 100)";
            }
            catch (Exception ex)
            {
                if (throwOnError)
                    throw ex;
                return null;
            }
        }

        public override string GetText()
        {
            string[] vals = GetValues(false);
            if (vals.Length == 0) return htmlPrefix + emptyValueText + htmlPostfix;
            return base.GetText();
        }

        public override string GetShortText()
        {
            string[] vals = GetValues(false);
            if (vals.Length == 0) return shortTextPrefix + emptyValueText + shortTextPostfix;
            return base.GetText();
        }

        public override string GetHTML()
        {
            string[] values = GetValues(false);
            string s = "";
            var resources = new ResourceManager(typeof (ListOption));
            s = values.Length == 0
                    ? emptyValueText
                    : values.Aggregate(s,
                                       (current, t) =>
                                       current +
                                       ((current.Length > 0
                                             ? (Mode == Modes.AND
                                                    ? resources.GetString("And")
                                                    : resources.GetString("Or"))
                                             : "") + GetItemText(t)));

            return Regex.Replace((GetValues(false).Length > 1 ? htmlPrefix2 : htmlPrefix), "[ ]$", "");
        }
    }
}