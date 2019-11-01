using System.Text.RegularExpressions;
using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns
{
    public class ValueOption : MyOption
    {
        protected string mask;

        protected ValueOption(XmlElement el) : base(el)
        {
        }

        public override string GetHTML()
        {
            string s = Value.Length == 0 ? emptyValueText : GetItemText(Value);
            return Regex.Replace(htmlPrefix, "[ ]$", ": ") +
                   htmlLinkPrefix +
                   htmlItemPrefix +
                   s +
                   htmlItemPostfix +
                   htmlLinkPostfix +
                   htmlPostfix;
        }

        public override string GetText()
        {
            return string.IsNullOrEmpty(Value)
                       ? string.Empty
                       : htmlPrefix + textItemPrefix + GetItemText(Value) + textItemPostfix + htmlPostfix;
        }

        public override string GetShortText()
        {
            return string.IsNullOrEmpty(Value)
                       ? string.Empty
                       : shortTextPrefix + textItemPrefix + GetItemText(Value) + textItemPostfix + shortTextPostfix;
        }
    }
}
