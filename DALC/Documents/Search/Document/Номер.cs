using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;
using Kesco.Lib.Win.Data.Types;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Document
{
    [Option("Номер", typeof (Номер))]
    public class Номер : TextOption
    {
        private bool noNumber;

        protected Номер(XmlElement el)
            : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPostfix = "";

            textItemPrefix = "'";
            textItemPostfix = "'";

            noNumber = el.GetAttribute("mode").ToLower().Equals("no");
        }

        public override Modes Mode
        {
            get { return Modes.Contains; }
        }

        public override string GetSQL(bool throwOnError)
        {
            if (Value.Length == 0)
            {
                if (noNumber)
                    return "T0.НомерДокумента = ''";

                if (throwOnError)
                    throw new Exception(Resources.GetString("GetSQL"));
                return null;
            }

            return GetSQLCondition("T0.НомерДокументаRL");
        }

        public override string GetShortText()
        {
            if (noNumber)
                return Resources.GetString("noNumber");
            return base.GetShortText();
        }

        public override string GetHTML()
        {
            if (noNumber)
            {
                return htmlPrefix +
                       htmlLinkPrefix +
                       htmlItemPrefix +
                       Resources.GetString("absent") +
                       htmlItemPostfix +
                       htmlLinkPostfix +
                       htmlPostfix;
            }

            return base.GetHTML();
        }

        public override string GetText()
        {
            if (noNumber)
            {
                return htmlPrefix +
                       textItemPrefix + Resources.GetString("absent") + textItemPostfix +
                       htmlPostfix;
            }
            return base.GetText();
        }

        protected override string GetSQLCondition(string pattern)
        {
            string val = new KString(Value).SqlEscaped.Trim();

            return pattern + " LIKE '" + Replacer.ReplaceRusLat(val) + "%' OR " + pattern + "Reverse LIKE '" +
                   Env.ReverseString(Replacer.ReplaceRusLat(val)) + "%'";
        }
    }
}