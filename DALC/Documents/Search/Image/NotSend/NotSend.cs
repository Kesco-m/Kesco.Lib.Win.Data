using System;
using System.Text;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.NotSend
{
    [Option("Image.NotSend", ".")]
    public class NotSend : Option
    {
        private ДатаНеОтправлялось oDate;
        private НеОтправлялось oEmp;

        protected NotSend(XmlElement el)
            : base(el)
        {
            XmlElement el0;
            el0 = (XmlElement) el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.ДатаНеОтправлялось']");
            if (el0 != null)
                oDate = (ДатаНеОтправлялось) CreateOption(el0);

            el0 = (XmlElement) el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.НеОтправлялось']");
            if (el0 != null)
                oEmp = (НеОтправлялось) CreateOption(el0);
        }


        public override string GetHTML()
        {
            return string.Empty;
        }

        public override string GetSQL(bool throwOnError)
        {
            string s =
                @" EXISTS (SELECT *
FROM dbo.vwИзображенияДокументовlog T1 (NOLOCK)
WHERE T1.КодДокумента = T0.КодДокумента AND T1.Direction = 2";

            if (oDate != null)
                s += " AND " + oDate.GetSQLCondition2("T1.ВремяОтправки") + "";
            if (oEmp != null && oEmp.GetValues(false).Length > 0)
                if (oEmp.Mode == ListOption.Modes.AND)
                    s = "\nNOT (" + oEmp.GetSQLCondition2(s + " AND T1.КодСотрудника =@VAL" + ")") + ")";
                else
                    s = oEmp.GetSQLCondition2("\nNOT " + s + " AND T1.КодСотрудника =@VAL)").Replace("OR", "AND");
            else
            {
                s = "\nNOT " + s + ")";
            }

            return s;
        }

        public override string GetText()
        {
            var sb = new StringBuilder(Resources.GetString("ImageDocumentSentOut"));
            if (oDate != null)
                switch (oDate.Mode)
                {
                    case MinMaxOption.Modes.More:
                        sb.Append(" ");
                        sb.Append(Resources.GetString("NotBefore"));
                        sb.Append(" ");
                        sb.Append(DateTime.Parse(oDate.Min).ToString("dd.MM.yyyy"));
                        break;
                    case MinMaxOption.Modes.Less:
                        sb.Append(" ");
                        sb.Append(Resources.GetString("NotAfter"));
                        sb.Append(" ");
                        sb.Append(DateTime.Parse(oDate.Max).ToString("dd.MM.yyyy"));
                        break;
                    case MinMaxOption.Modes.Equals:
                        sb.Append(" ");
                        sb.Append(DateTime.Parse(oDate.Min).ToString("dd.MM.yyyy"));
                        break;
                    case MinMaxOption.Modes.Interval:
                        sb.Append(" ");
                        sb.Append(Resources.GetString("Between"));
                        sb.Append(" ");
                        sb.Append(DateTime.Parse(oDate.Min).ToString("dd.MM.yyyy"));
                        sb.Append(" ");
                        sb.Append(Resources.GetString("To"));
                        sb.Append(" ");
                        sb.Append(DateTime.Parse(oDate.Max).ToString("dd.MM.yyyy"));
                        break;
                }
            if (oEmp != null)
            {
                sb.Append(" ");
                sb.Append(oEmp.GetItemsText(2, "", ""));
            }

            return sb.ToString();
        }

        public override string GetShortText()
        {
            string s = Resources.GetString("SentOut");
            if (oDate != null)
                switch (oDate.Mode)
                {
                    case MinMaxOption.Modes.More:
                        s += " " + Resources.GetString("NotBefore") + " " +
                             DateTime.Parse(oDate.Min).ToString("dd.MM.yyyy");
                        break;
                    case MinMaxOption.Modes.Less:
                        s += " " + Resources.GetString("NotAfter") + " " +
                             DateTime.Parse(oDate.Max).ToString("dd.MM.yyyy");
                        break;
                    case MinMaxOption.Modes.Equals:
                        s += " " + DateTime.Parse(oDate.Min).ToString("dd.MM.yyyy");
                        break;
                    case MinMaxOption.Modes.Interval:
                        s += " " + Resources.GetString("Between") + " " +
                             DateTime.Parse(oDate.Min).ToString("dd.MM.yyyy") +
                             " " + Resources.GetString("To") + " " + DateTime.Parse(oDate.Max).ToString("dd.MM.yyyy");
                        break;
                }
            if (oEmp != null)
            {
                s += " " + oEmp.GetItemsText(2, "", "");
            }

            return s;
        }
    }
}
