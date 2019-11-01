using System;
using System.Resources;
using System.Text;
using System.Threading;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Message.Incoming
{
    [Option("Message.Incoming", ".")]
    public class Incoming : Option
    {
        private Incoming_By oBy;
        private Incoming_Read oRead;
        private Incoming_Unread oUnread;
        private Incoming_Date oDate;
        private Incoming_Text oText;
        private Incoming_Chief oChief;
        private Incoming_Employee oEmp;

        protected Incoming(XmlElement el) : base(el)
        {
            XmlElement el0;
            el0 = (XmlElement) el.OwnerDocument.SelectSingleNode("Options/Option[@name='Message.Incoming.By']");
            if (el0 != null) 
                oBy = (Incoming_By) CreateOption(el0);

            el0 = (XmlElement) el.OwnerDocument.SelectSingleNode("Options/Option[@name='Message.Incoming.Read']");
            if (el0 != null) 
                oRead = (Incoming_Read) CreateOption(el0);

            el0 = (XmlElement) el.OwnerDocument.SelectSingleNode("Options/Option[@name='Message.Incoming.Unread']");
            if (el0 != null)
                oUnread = (Incoming_Unread) CreateOption(el0);

            el0 = (XmlElement) el.OwnerDocument.SelectSingleNode("Options/Option[@name='Message.Incoming.Date']");
            if (el0 != null)
                oDate = (Incoming_Date) CreateOption(el0);

            el0 = (XmlElement) el.OwnerDocument.SelectSingleNode("Options/Option[@name='Message.Incoming.Text']");
            if (el0 != null)
                oText = (Incoming_Text) CreateOption(el0);

            el0 = (XmlElement) el.OwnerDocument.SelectSingleNode("Options/Option[@name='Message.Incoming.Chief']");
            if (el0 != null) 
                oChief = (Incoming_Chief) CreateOption(el0);

            el0 = (XmlElement) el.OwnerDocument.SelectSingleNode("Options/Option[@name='Message.Incoming.Employee']");
            if (el0 != null)
                oEmp = (Incoming_Employee) CreateOption(el0);
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
                FROM Документы.dbo.vwСообщенияВходящие TI WITH(NOLOCK)
                WHERE TI.КодДокумента=T0.КодДокумента";

            if (oBy != null && oBy.GetValues(false).Length > 0)
                s += " AND TI.КодСотрудникаОтправителя IN (" + oBy.Value + ")";
            if (oChief != null)
                s +=
                    " AND TI.КодСотрудникаОтправителя IN (SELECT КодСотрудника FROM Инвентаризация.dbo.vwРуководители VCh WHERE VCh.КодСотрудника <> " +
                    oChief.Value + ")";
            if (oEmp != null)
                s +=
                    " AND TI.КодСотрудникаОтправителя IN (SELECT КодСотрудника FROM Инвентаризация.dbo.vwПодчинённые VP WHERE VP.КодСотрудника <> " +
                    oEmp.Value + ")";
            if (oDate != null)
                s += " AND (" + oDate.GetSQLCondition2("Отправлено") + ")";
            if (oText != null)
                s += " AND " + oText.GetSQLCondition2("TI.Сообщение");
            if (oRead != null)
                s += " AND TI.Прочитано IS NOT NULL";
            if (oUnread != null)
                s += " AND TI.Прочитано IS NULL";
            s += ")";

            return s;
        }

        public override string GetText()
        {
            var sb = new StringBuilder(Resources.GetString("IncomingMessage"));
            if (oBy != null && oBy.GetValues(false).Length > 0)
            {
                sb.Append(" ");
                sb.Append(Resources.GetString("From"));
                sb.Append(" ");
                sb.Append(oBy.GetItemsText());
            }
            if (oDate != null)
            {
				var dRes = new ResourceManager(typeof(DateOption));
                string td1 = dRes.GetString("Today");
                string td = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == "ru" ? "сегодня" : td1;
                switch (oDate.Mode)
                {
					case MinMaxOption.Modes.More:
                        sb.Append(" ");
                        sb.Append(Resources.GetString("NotBefore"));
                        sb.Append(" ");
                        sb.Append(oDate.Min.Equals(td1) ? td : DateTime.Parse(oDate.Min).ToString("dd.MM.yyyy"));
                        break;
					case MinMaxOption.Modes.Less:
                        sb.Append(" ");
                        sb.Append(Resources.GetString("NotAfter"));
                        sb.Append(" ");
                        sb.Append(oDate.Max.Equals(td1) ? td : DateTime.Parse(oDate.Max).ToString("dd.MM.yyyy"));
                        break;
					case MinMaxOption.Modes.Equals:
                        sb.Append(" ");
                        if (oDate.Min.Equals(td1)) sb.Append(td);
                        else sb.Append(DateTime.Parse(oDate.Min).ToString("dd.MM.yyyy"));
                        break;
					case MinMaxOption.Modes.Interval:
                        sb.Append(" ");
                        sb.Append(Resources.GetString("Between"));
                        sb.Append(" ");
                        sb.Append(oDate.Min.Equals(td1) ? td : DateTime.Parse(oDate.Min).ToString("dd.MM.yyyy"));
                        sb.Append(" ");
                        sb.Append(Resources.GetString("To"));
                        sb.Append(" ");
                        sb.Append(oDate.Max.Equals(td1) ? td : DateTime.Parse(oDate.Max).ToString("dd.MM.yyyy"));
                        break;
                }
            }
            if (oText != null && oText.Value.Length > 0)
                switch (oText.Mode)
                {
					case TextOption.Modes.BeginsWith:
                        sb.Append(" ");
                        sb.Append(Resources.GetString("StartingWith"));
                        sb.Append(" '");
                        sb.Append(oText.Value);
                        sb.Append("'");
                        break;
                    case TextOption.Modes.Contains:
                        sb.Append(" ");
                        sb.Append(Resources.GetString("Containing"));
                        sb.Append(" '");
                        sb.Append(oText.Value);
                        sb.Append("'");
                        break;
                }
            if (oRead != null)
                sb.Append(Resources.GetString("Reading"));
            if (oUnread != null)
                sb.Append(Resources.GetString("NotReading"));

            return sb.ToString();
        }

        public override string GetShortText()
        {
            string s = Resources.GetString("IncomingMessage");
            if (oBy != null && oBy.GetValues(false).Length > 0)
                s += " " + Resources.GetString("From") + " " + oBy.GetItemsText(2, "", "");
            if (oDate != null)
            {
				var dRes = new ResourceManager(typeof(DateOption));
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
            if (oRead != null)
                s += Resources.GetString("Reading");
            if (oUnread != null)
                s += Resources.GetString("NotReading");

            return s;
        }
    }
}
