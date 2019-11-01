using System;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;
using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns
{
    /// <summary>
    /// Summary description for TwoValueOption.
    /// </summary>
    public class TwoListValueOption : ListOption
    {
        protected string htmlSplitter;
        protected string htmlSplitter2;
        protected string shortTextSplitter;
        protected string emptyValueText2;
        protected string htmlLinkPrefix2;


        protected TwoListValueOption(XmlElement el) : base(el)
        {
            htmlLinkPrefix2 = "[<A href=#" + Meta.Name + ";Value2>";
        }

        public string Value2
        {
            get { return el.GetAttribute("value2"); }
        }

        public virtual void SetValue2(string newValue)
        {
            string s = newValue.Trim();
            if (s.Length == 0) el.RemoveAttribute("value2");
            el.SetAttribute("value2", s);

        }

        public override string GetHTML()
        {
            string[] values = GetValues(false);
            string[] values2 = GetValues2(false);
            string s = "";
            string s1 = "";
            var resources = new ResourceManager(typeof (ListOption));
            s = values.Length == 0 ? emptyValueText : values.Aggregate(s, (current, t) => current + ((current.Length > 0 ? (Mode == Modes.AND ? resources.GetString("And") : resources.GetString("Or")) : "") + htmlItemPrefix + GetItemText(t) + htmlItemPostfix));

            s1 = values2.Length == 0 ? emptyValueText2 : values2.Aggregate(s1, (current, t) => current + ((current.Length > 0 ? (Mode == Modes.AND ? resources.GetString("And") : resources.GetString("Or")) : "") + htmlItemPrefix + GetItemText(t) + htmlItemPostfix));


            return Regex.Replace((GetValues(false).Length > 1 ? htmlPrefix2 : htmlPrefix), "[ ]$", ": ") +
                   htmlLinkPrefix +
                   s +
                   htmlLinkPostfix +
                   htmlSplitter +
                   htmlLinkPrefix2 +
                   s1 +
                   htmlLinkPostfix +

                   htmlPostfix;
        }

        public override string GetText()
        {
            string[] values = GetValues(false);
            string[] values2 = GetValues2(false);
            string s = "";
            string s1 = "";
            var resources = new ResourceManager(typeof (ListOption));
            s = values.Length == 0 ? emptyValueText : values.Aggregate(s, (current, t) => current + ((current.Length > 0 ? (Mode == Modes.AND ? resources.GetString("And") : resources.GetString("Or")) : "") + htmlItemPrefix + GetItemText(t) + htmlItemPostfix));

            s1 = values2.Length == 0 ? emptyValueText2 : values2.Aggregate(s1, (current, t) => current + ((current.Length > 0 ? (Mode == Modes.AND ? resources.GetString("And") : resources.GetString("Or")) : "") + htmlItemPrefix + GetItemText(t) + htmlItemPostfix));

            return htmlPrefix + textItemPrefix + s + textItemPostfix + htmlSplitter + textItemPrefix + s1 +
                   textItemPostfix + htmlPostfix;
        }

        public override string GetShortText()
        {
            return string.IsNullOrEmpty(Value)
                       ? string.Empty
                       : shortTextPrefix + textItemPrefix + GetItemsText(3, "", "") + textItemPostfix +
                         shortTextSplitter +
                         textItemPrefix + GetItemsText2(3, "", "") + textItemPostfix + shortTextPostfix;
        }

        protected string GetSQLConditionFor2(string pattern)
        {
            string[] values = GetValues2(false);
            return values.Aggregate("", (current, t) => current + ((current.Length > 0 ? (Mode == Modes.AND ? " AND " : " OR ") : "") + "(" + pattern.Replace("@VAL", GetSQLParameter(t)) + ")"));
        }

        public string[] GetValues2(bool throwOnError)
        {
            if (Value2.Length == 0) 
                return new string[] {};

            string[] S = Value2.Split(',');

            foreach (string t in S.Where(t => mask != null && !Regex.IsMatch(t, mask, RegexOptions.IgnoreCase)))
                if (throwOnError)
                {
                    var resources = new ResourceManager(typeof (ListOption));
                    throw new Exception(resources.GetString("GetValues") + mask + ".");
                }
                else 
                    return new string[] {};

            return S;
        }

        public string GetItemsText2(int maxNumber, string itemPrefix, string itemPostfix)
        {
            string[] val = GetValues2(false);

            string s = "";
            var resources = new ResourceManager(typeof (ListOption));
            string w = (Mode == Modes.AND ? resources.GetString("And") : resources.GetString("Or"));
            string ate = resources.GetString("Other");
            for (int i = 0; i < val.Length; i++)
            {
                if (i < maxNumber || maxNumber == -1)
                    s += (s.Length > 0 ? w : "") + itemPrefix + GetItemText(val[i]) + itemPostfix;
                else if (i == maxNumber)
                    s += w + ate;
            }
            return s;
        }
    }
}

