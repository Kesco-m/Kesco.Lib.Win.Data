using System;
using System.Resources;
using System.Threading;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Message.Outgoing
{
    [Option("Message.Outgoing", ".")]
    public class Outgoing : Option
    {
        private Outgoing_By oBy;
        private Outgoing_Date oDate;
        private Outgoing_Text oText;

        protected Outgoing(XmlElement el) : base(el)
        {
            XmlElement el0;
            el0 = (XmlElement) el.OwnerDocument.SelectSingleNode("Options/Option[@name='Message.Outgoing.By']");
            if (el0 != null) oBy = (Outgoing_By) CreateOption(el0);

            el0 = (XmlElement) el.OwnerDocument.SelectSingleNode("Options/Option[@name='Message.Outgoing.Date']");
            if (el0 != null) oDate = (Outgoing_Date) CreateOption(el0);

            el0 = (XmlElement) el.OwnerDocument.SelectSingleNode("Options/Option[@name='Message.Outgoing.Text']");
            if (el0 != null) oText = (Outgoing_Text) CreateOption(el0);
        }

        public override string GetHTML()
        {
            return string.Empty;
        }

        public override string GetSQL(bool throwOnError)
        {
            string s =
                @"
                EXISTS (SELECT *
                FROM Документы.dbo.vwСообщенияИсходящие TI WITH(NOLOCK)
                WHERE TI.КодДокумента=T0.КодДокумента";
            if (oDate != null)
                s += " AND (" + oDate.GetSQLCondition2("Отправлено") + ")";

            if (oBy != null && oBy.GetValues(false).Length > 0)
                s += " AND TI.КодСотрудникаПолучателя IN (" + oBy.Value + ")";
            if (oText != null)
                s += " AND " + oText.GetSQLCondition2("TI.Сообщение");
            s += ")";

            return s;
        }

        public override string GetText()
        {
            string s = Resources.GetString("SentMessage");
            if (oBy != null && oBy.GetValues(false).Length > 0)
                s += " " + oBy.GetItemsText();
            if (oDate != null)
            {
                var dRes = new ResourceManager(typeof (DateOption));
                string td, td1 = dRes.GetString("Today");
                td = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ru" ? "сегодня" : td1;
                switch (oDate.Mode)
                {
                    case MinMaxOption.Modes.More:
                        if (oDate.Min.Equals(td1))
                            s += " " + Resources.GetString("NotBefore") + " " + td;
                        else
                            s += " " + Resources.GetString("NotBefore") + " " +
                                 DateTime.Parse(oDate.Min).ToString("dd.MM.yyyy");
                        break;
                    case MinMaxOption.Modes.Less:
                        if (oDate.Max.Equals(td1))
                            s += " " + Resources.GetString("NotAfter") + " " + td;
                        else
                            s += " " + Resources.GetString("NotAfter") + " " +
                                 DateTime.Parse(oDate.Max).ToString("dd.MM.yyyy");
                        break;
                    case MinMaxOption.Modes.Equals:
                        if (oDate.Min.Equals(td1))
                            s += " " + td;
                        else
                            s += " " + DateTime.Parse(oDate.Min).ToString("dd.MM.yyyy");
                        break;
                    case MinMaxOption.Modes.Interval:
                        string from = oDate.Min;
                        @from = @from.Equals(td1) ? td : DateTime.Parse(oDate.Min).ToString("dd.MM.yyyy");
                        string to = oDate.Max;
                        to = to.Equals(td1) ? td : DateTime.Parse(oDate.Max).ToString("dd.MM.yyyy");
                        s += " " + Resources.GetString("Between") + " " + from + " " + Resources.GetString("To") + " " +
                             to;
                        break;
                }
            }
            if (oText != null && oText.Value.Length > 0)
                switch (oText.Mode)
                {
                    case TextOption.Modes.BeginsWith:
                        s += " " + Resources.GetString("StartingWith") + " '" + oText.Value + "'";
                        break;
                    case TextOption.Modes.Contains:
                        s += " " + Resources.GetString("Containing") + " '" + oText.Value + "'";
                        break;
                }


            return s;
        }

        public override string GetShortText()
        {
            string s = Resources.GetString("SentMessage");
            if (oBy != null && oBy.GetValues(false).Length > 0)
                s += " " + oBy.GetItemsText(2, "", "");
            if (oDate != null)
            {
                var dRes = new ResourceManager(typeof (DateOption));
                string td = dRes.GetString("Today");
                switch (oDate.Mode)
                {
                    case MinMaxOption.Modes.More:
                        if (oDate.Min.Equals(td))
                            s += " " + Resources.GetString("NotBefore") + " " + td;
                        else
                            s += " " + Resources.GetString("NotBefore") + " " +
                                 DateTime.Parse(oDate.Min).ToString("dd.MM.yyyy");
                        break;
                    case MinMaxOption.Modes.Less:
                        if (oDate.Max.Equals(td))
                            s += " " + Resources.GetString("NotAfter") + " " + td;
                        else
                            s += " " + Resources.GetString("NotAfter") + " " +
                                 DateTime.Parse(oDate.Max).ToString("dd.MM.yyyy");
                        break;
                    case MinMaxOption.Modes.Equals:
                        if (oDate.Min.Equals(td))
                            s += " " + td;
                        else
                            s += " " + DateTime.Parse(oDate.Min).ToString("dd.MM.yyyy");
                        break;
                    case MinMaxOption.Modes.Interval:
                        string from = oDate.Min;
                        @from = @from.Equals(td) ? td : DateTime.Parse(oDate.Min).ToString("dd.MM.yyyy");
                        string to = oDate.Max;
                        to = to.Equals(td) ? td : DateTime.Parse(oDate.Max).ToString("dd.MM.yyyy");
                        s += " " + Resources.GetString("Between") + " " + from + " " + Resources.GetString("To") + " " +
                             to;
                        break;
                }
            }
            if (oText != null && oText.Value.Length > 0)
                switch (oText.Mode)
                {
                    case TextOption.Modes.BeginsWith:
                        s += " " + Resources.GetString("StartingWith") + " '" + oText.Value + "'";
                        break;
                    case TextOption.Modes.Contains:
                        s += " " + Resources.GetString("Containing") + " '" + oText.Value + "'";
                        break;
                }

            return s;
        }
    }
}
