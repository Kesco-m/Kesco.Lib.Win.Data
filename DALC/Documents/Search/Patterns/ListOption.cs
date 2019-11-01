using System;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.New_Search.Base;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns
{
    public class ListOption : ValueOption
    {
        protected string htmlPrefix2;

        protected ListOption(XmlElement el) : base(el)
        {
            mask = "^[1-9][0-9]{0,9}$"; //чиселка
        }

        public enum Modes
        {
            OR, //default
            AND
        }

        public Modes Mode
        {
            get { return el.GetAttribute("mode").ToLower().Equals("and") ? Modes.AND : Modes.OR; }
            set
            {
                switch (value)
                {
                    case Modes.AND:
                        el.SetAttribute("mode", "and");
                        break;
                    default:
                        el.SetAttribute("mode", "or");
                        break;
                }
            }
        }

        public virtual string[] GetValues(bool throwOnError)
        {
            if (Value.Length == 0) return new string[] {};

            string[] S = Value.Split(',');

            foreach (string t in S.Where(t => mask != null && !Regex.IsMatch(t, mask, RegexOptions.IgnoreCase)))
                if (throwOnError)
                {
                    var resources = new ResourceManager(typeof (ListOption));
                    throw new Exception(resources.GetString("GetValues") + mask + ".");
                }
                else return new string[] {};

            return S;
        }

        public string GetSQLCondition2(string field)
        {
            return GetSQLCondition(field);
        }

        protected override string GetSQLCondition(string pattern)
        {
            string[] values = GetValues(false);
            return values.Aggregate("", (current, t) => current + ((current.Length > 0 ? (Mode == Modes.AND ? " AND " : " OR ") : "") + "(" + pattern.Replace("@VAL", GetSQLParameter(t)) + ")"));
        }

        public override string GetHTML()
        {
            string[] values = GetValues(false);
            string s = "";
            var resources = new ResourceManager(typeof (ListOption));
			s = values.Length == 0 ? htmlItemPrefix + emptyValueText + htmlItemPostfix : values.Aggregate(s, (current, t) => current + ((current.Length > 0 ? (Mode == Modes.AND ? resources.GetString("And") : resources.GetString("Or")) : "") + htmlItemPrefix + GetItemText(t) + htmlItemPostfix));

            return Regex.Replace((GetValues(false).Length > 1 ? htmlPrefix2 : htmlPrefix), "[ ]$", ": ") +
                   htmlLinkPrefix +
                   s +
                   htmlLinkPostfix +
                   htmlPostfix;
        }

        public string GetItemsText()
        {
            return GetItemsText(-1, textItemPrefix, textItemPostfix);
        }

        public string GetItemsText(int maxNumber, string itemPrefix, string itemPostfix)
        {
            string[] val = GetValues(false);

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

        public override string GetText()
        {
            return (GetValues(false).Length > 1 ? htmlPrefix2 : htmlPrefix) +
                   GetItemsText() +
                   htmlPostfix;
        }

        public override string GetShortText()
        {
            return shortTextPrefix + GetItemsText(2, "", "") + shortTextPostfix;
        }

		public bool GetSearchType()
		{
			AllAnyAttribute[ ] attr = (AllAnyAttribute[ ])this.GetType().GetCustomAttributes(typeof(AllAnyAttribute), false);
			return attr.Length == 1 ? attr[0].SearchType : false;
		}
    }
}
