using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kesco.Lib.Win.Data
{
    public struct DateSpan
    {
        private int days;
        private int weeks;
        private int months;
        private int years;

        #region ACCESSORS

        public int Days
        {
            get { return days; }
            set { days = value; }
        }

        public int Weeks
        {
            get { return weeks; }
            set { weeks = value; }
        }

        public int Months
        {
            get { return months; }
            set { months = value; }
        }

        public int Years
        {
            get { return years; }
            set { years = value; }
        }

        #endregion

        public static DateSpan Parse(string s)
        {
            var ds = new DateSpan();
            foreach (Match m in from Match m in Regex.Matches(s, "([0-9]+)([YMWD])") where m.Success select m)
            {
                switch (m.Groups[2].Value)
                {
                    case "Y":
                        ds.years = int.Parse(m.Groups[1].Value);
                        break;
                    case "M":
                        ds.months = int.Parse(m.Groups[1].Value);
                        break;
                    case "W":
                        ds.weeks = int.Parse(m.Groups[1].Value);
                        break;
                    case "D":
                        ds.days = int.Parse(m.Groups[1].Value);
                        break;
                }
            }
            return ds;
        }

        public static DateSpan ParseText(string s)
        {
            var ds = new DateSpan();
            foreach (
                Match m in
                    from Match m in Regex.Matches(s.ToUpper(), "([0-9]+)[ ]{0,}([ГЛМНД]|$)") where m.Success select m)
            {
                switch (m.Groups[2].Value)
                {
                    case "Г":
                    case "Л":
                        ds.years = int.Parse(m.Groups[1].Value);
                        break;
                    case "М":
                        ds.months = int.Parse(m.Groups[1].Value);
                        break;
                    case "Н":
                        ds.weeks = int.Parse(m.Groups[1].Value);
                        break;
                    default:
                        ds.days = int.Parse(m.Groups[1].Value);
                        break;
                }
            }
            return ds;
        }

        public override string ToString()
        {
            string s = "";
            if (years > 0) s += years.ToString() + "Y";
            if (months > 0) s += months.ToString() + "M";
            if (weeks > 0) s += weeks.ToString() + "W";
            if (days > 0) s += days.ToString() + "D";
            return s;
        }

        public DateSpan(int years, int months, int weeks, int days)
        {
            this.years = years;
            this.months = months;
            this.weeks = weeks;
            this.days = days;
        }

        private static string[] _y = new[] {"год", "года", "лет"};
        private static string[] _m = new[] {"месяц", "месяца", "месяцев"};
        private static string[] _w = new[] {"неделя", "недели", "недель"};
        private static string[] _d = new[] {"день", "дня", "дней"};

        private string GetCaption(int i, string[] S)
        {
            if (i > 10 && i < 20) return S[2];

            switch (i%10)
            {
                case 1:
                    return S[0];
                case 2:
                case 3:
                case 4:
                    return S[1];
                default:
                    return S[2];
            }
        }

        public string Text
        {
            get
            {
                string s = "";
                if (years > 0) s += years.ToString() + " " + GetCaption(years, _y);
                if (months > 0) s += (s.Length > 0 ? " " : "") + months.ToString() + " " + GetCaption(months, _m);
                if (weeks > 0) s += (s.Length > 0 ? " " : "") + weeks.ToString() + " " + GetCaption(weeks, _w);
                if (days > 0) s += (s.Length > 0 ? " " : "") + days.ToString() + " " + GetCaption(days, _d);
                return s;
            }
        }

        public DateTime Add(DateTime dt)
        {
            if (years > 0) dt = dt.AddYears(years);
            if (months > 0) dt = dt.AddMonths(months);
            if (weeks > 0) dt = dt.AddDays(weeks*7);
            if (days > 0) dt = dt.AddDays(days);
            return dt;
        }
    }
}
