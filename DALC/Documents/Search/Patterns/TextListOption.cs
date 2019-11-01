using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;
using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns
{
    /// <summary>
    /// Summary description for TextListOption.
    /// </summary>
    public class TextListOption : ListOption
    {
        public TextListOption(XmlElement el) : base(el)
        {
            mask = "^[^']{1,}$";
        }

        public override string GetHTML()
        {
            string[] values = GetValues(false);
            string s = "";
            var resources = new ResourceManager(typeof (ListOption));
            s = values.Length == 0 ? emptyValueText : values.Aggregate(s, (current, t) => current + ((current.Length > 0 ? (Mode == Modes.AND ? resources.GetString("And") : resources.GetString("Or")) : "") + htmlItemPrefix + "'" + GetItemText(t) + "'" + htmlItemPostfix));

            return Regex.Replace((GetValues(false).Length > 1 ? htmlPrefix2 : htmlPrefix), "[ ]$", ": ") +
                   htmlLinkPrefix +
                   s +
                   htmlLinkPostfix +
                   htmlPostfix;
        }
    }
}
