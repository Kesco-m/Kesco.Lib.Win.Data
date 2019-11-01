using System;
using System.Resources;
using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns
{
    public class DateOption : MinMaxOption
    {
		
        protected DateOption(XmlElement el) : base(el)
        {
			
        }

        public new string GetSQLCondition2(string pattern)
        {
            return GetSQLCondition(pattern);
        }

        protected override string GetSQLCondition(string field)
        {
            DateTime min, max;
            switch (Mode)
            {
                case Modes.More:
                    min = base.Min.Equals("Today") ? DateTime.Today : DateTime.Parse(Min);
                    return field + ">='" + min.ToString("yyyyMMdd") + "'";

                case Modes.Less:
                    max = base.Max.Equals("Today") ? DateTime.Today : DateTime.Parse(Max);
                    return field + "<'" + max.AddDays(1).ToString("yyyyMMdd") + "'";

                case Modes.Equals:
                    min = base.Min.Equals("Today") ? DateTime.Today : DateTime.Parse(Min);
                    return field + ">='" + min.ToString("yyyyMMdd") + "' AND " +
                           field + "<'" + min.AddDays(1).ToString("yyyyMMdd") + "'";

                case Modes.Interval:
                    min = DateTime.Parse(Min);
                    max = DateTime.Parse(Max);
                    return field + ">='" + min.ToString("yyyyMMdd") + "' AND " +
                           field + "<'" + max.AddDays(1).ToString("yyyyMMdd") + "'";

                default:
					if(HasEmpty())
						return field + " IS NULL";
					else
					{
						var resources = new ResourceManager(typeof(DateOption));
						throw new Exception(resources.GetString("GetSQLCondition.Error1") + Meta.Description +
											resources.GetString("GetSQLCondition.Error2"));
					}
            }
        }

        public override string Min
        {
            get
            {
                string res = base.Min;
                if (!string.IsNullOrEmpty(res) && res.Equals("Today"))
                {
                    var resources = new ResourceManager(typeof (DateOption));
                    return resources.GetString("Today");
                }
                return res;
            }
        }

        public override string Max
        {
            get
            {
                string res = base.Max;
                if (!string.IsNullOrEmpty(res) && res.Equals("Today"))
                {
                    var resources = new ResourceManager(typeof (DateOption));
                    return resources.GetString("Today");
                }
                return res;
            }
        }
    }
}
