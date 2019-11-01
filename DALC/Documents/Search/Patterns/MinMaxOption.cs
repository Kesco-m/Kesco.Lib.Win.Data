using System;
using System.Resources;
using System.Text.RegularExpressions;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.New_Search.Base;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns
{
    public class MinMaxOption : Option
    {
		
        public enum Modes
        {
            None,
            More, //min>=
            Less, //<=max
            Equals, //min==max!=""
            Interval
        }

		
        protected MinMaxOption(XmlElement el) : base(el)
        {
		}

		public virtual string Min
        {
            get { return el.GetAttribute("min"); }
        }

        public virtual string Max
        {
            get { return el.GetAttribute("max"); }
        }

        public Modes Mode
        {
            get
            {
                int flags = (Min.Length > 0 ? 0x0001 : 0) +
                            (Max.Length > 0 ? 0x0010 : 0) +
                            (Min.Equals(Max) ? 0x0100 : 0);
                switch (flags)
                {
                    case 0x0001:
                        return Modes.More;
                    case 0x0010:
                        return Modes.Less;
                    case 0x0111:
                        return Modes.Equals;
                    case 0x0011:
                        return Modes.Interval;
                    default:
                        return Modes.None;
                }
            }
        }

		public string GetSQLCondition2(string field)
		{
			return GetSQLCondition(field);
		}

        protected override string GetSQLCondition(string field)
        {
            switch (Mode)
            {
                case Modes.More:
                    return field + ">= " + GetSQLParameter(Min) + " ";
                case Modes.Less:
                    return field + "<= " + GetSQLParameter(Max) + " ";
                case Modes.Equals:
                    return field + "= " + GetSQLParameter(Min) + " ";
                case Modes.Interval:
                    return field + ">= " + GetSQLParameter(Min) + " AND " + field + "<=" + GetSQLParameter(Max) + " ";
                default:
                    var resources = new ResourceManager(typeof (MinMaxOption));
                    throw new Exception(resources.GetString("GetSQLCondition.Error1") + Meta.Description +
                                        resources.GetString("GetSQLCondition.Error2"));
            }
        }

        public override string GetItemText(string key)
        {
            var resources = new ResourceManager(typeof (MinMaxOption));
            switch (Mode)
            {
                case Modes.More:
                    return resources.GetString("More") + Min;
                case Modes.Less:
                    return resources.GetString("Less") + Max;
                case Modes.Equals:
                    return resources.GetString("Equals") + Min;
                case Modes.Interval:
                    return resources.GetString("InInterval") + Min + " " + resources.GetString("To") + Max;
                default:
                    return emptyValueText;
            }
        }

        public virtual string GetShortTextItem()
        {
            var resources = new ResourceManager(typeof (MinMaxOption));

            switch (Mode)
            {
                case Modes.More:
                    return resources.GetString("From") + Min;
                case Modes.Less:
                    return resources.GetString("To") + Max;
                case Modes.Equals:
                    return Min;
                case Modes.Interval:
                    return resources.GetString("From") + Min + " " + resources.GetString("To") + Max;
                default:
                    return emptyValueText;
            }
        }

		public Type GetSearchType()
		{
			NumericTypeAttribute[] attr = (NumericTypeAttribute[])this.GetType().GetCustomAttributes(typeof(NumericTypeAttribute), false);
			return attr.Length == 1 ? attr[0].SearchType : null;
		}

        public override string GetHTML()
        {
            return Regex.Replace(htmlPrefix, "[ ]$", ": ") +
                   htmlLinkPrefix +
                   htmlItemPrefix +
                   GetItemText("") +
                   htmlItemPostfix +
                   htmlLinkPostfix +
                   htmlPostfix;
        }

        public override string GetText()
        {
			if(HasEmpty())
				return htmlPrefix + textItemPrefix + GetItemText("") + textItemPostfix + htmlPostfix;

            return Mode == Modes.None ? "" : htmlPrefix + textItemPrefix + GetItemText("") + textItemPostfix + htmlPostfix;
        }

        public override string GetShortText()
        {
            return Mode == Modes.None
                       ? ""
                       : shortTextPrefix + textItemPrefix + GetShortTextItem() + textItemPostfix + shortTextPostfix;
        }
    }
}
