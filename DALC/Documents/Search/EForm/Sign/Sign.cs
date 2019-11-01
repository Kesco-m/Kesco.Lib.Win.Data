using System;
using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm.Sign
{
    [Option("EForm.Sing", ".")]
    public class Sign : Option
    {
        private Выполнен oEnd;
        private ДатаВыполнения oDateEnd;
        private ДатаПодписания oDateSign;
        private Подписан oSign;
        private ПодписанМной oMySign;

        protected Sign(XmlElement el) : base(el)
        {
            XmlElement el0;
            el0 = (XmlElement) el.OwnerDocument.SelectSingleNode("Options/Option[@name='EForm.Sing.Выполнен']");
            if (el0 != null) oEnd = (Выполнен) CreateOption(el0);

            el0 = (XmlElement) el.OwnerDocument.SelectSingleNode("Options/Option[@name='EForm.Sing.ДатаВыполнения']");
            if (el0 != null) oDateEnd = (ДатаВыполнения) CreateOption(el0);
        }

        public override string GetHTML()
        {
            return string.Empty;
        }

        public override string GetSQL(bool throwOnError)
        {
            bool open = false;
            string s =
                @"
EXISTS (SELECT *
FROM Документы.dbo.ПодписиДокументов TI WITH(NOLOCK)
WHERE TI.КодДокумента=T0.КодДокумента AND TI.КодИзображенияДокумента IS NULL ";

            if (oEnd != null)
            {
                s += " AND ((TI.ТипПодписи=1";
                open = true;
                try
                {
                    if (oEnd.GetValues(false).Length > 0)
                        s += " AND (" + oEnd.GetSQLCondition2("TI.КодСотрудникаЗа = @VAL OR TI.КодСотрудника = @VAL") +
                             ")";
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex);
                }
                if (oDateEnd != null)
                    s += " AND (" + oDateEnd.GetSQLCondition2("TI.Дата") + ")";

                s += ")";
            }
            else if (oDateEnd != null)
                s += " AND (TI.ТипПодписи=1 AND " + oDateEnd.GetSQLCondition2("TI.Дата") + ")";
            if (open)
                s += ")";
            s += ")\n";

            return s;
        }
    }
}
