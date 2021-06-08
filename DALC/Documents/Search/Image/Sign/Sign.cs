using System;
using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.Sign
{
    /// <summary>
    /// Summary description for Sing.
    /// </summary>
    [Option("Image.Sign", ".")]
    public class Sign : Option
    {
        private Аннулировано oEnd;
        private ДатаАннулирования oDateEnd;

        protected Sign(XmlElement el) : base(el)
        {
            XmlElement el0;
            el0 = (XmlElement) el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.Sing.Аннулировано']");
            if (el0 != null) oEnd = (Аннулировано) CreateOption(el0);

            el0 = (XmlElement) el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.Sing.ДатаАннулирования']");
            if (el0 != null) oDateEnd = (ДатаАннулирования) CreateOption(el0);
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
FROM Документы.dbo.ПодписиДокументов TI WITH(NOLOCK)
WHERE TI.КодДокумента=T0.КодДокумента AND TI.КодИзображенияДокумента IS NOT NULL ";

			if(oEnd != null || oDateEnd != null)
			{
				s += " AND (TI.ТипПодписи=2";
				try
				{
					if(oEnd.GetValues(false).Length > 0)
						s += " AND (" + oEnd.GetSQLCondition2("TI.КодСотрудникаЗа = @VAL OR TI.КодСотрудника = @VAL") +  ")";
				}
				catch(Exception ex)
				{
					Env.WriteToLog(ex);
				}
				if(oDateEnd != null)
					s += " AND (" + oDateEnd.GetSQLCondition2("TI.Дата") + ")";

				s += ")\n";
			}
			s += ")\n";
			if((oEnd != null && !oEnd.IsSeparate())|| (oDateEnd != null && !oDateEnd.IsSeparate()))
			{
				s+=@"OR EXISTS(SELECT * FROM Документы.dbo.ПодписиДокументов TP WITH(NOLOCK)
		WHERE TP.КодДокумента=T0.КодДокумента AND TP.ТипПодписи=1 AND TP.КодИзображенияДокумента IS NULL";
				try
				{
					if(oEnd.GetValues(false).Length > 0)
						s += " AND (" + oEnd.GetSQLCondition2("TP.КодСотрудникаЗа = @VAL OR TP.КодСотрудника = @VAL") +  ")";
				}
				catch(Exception ex)
				{
					Env.WriteToLog(ex);
				}
				if(oDateEnd != null)
					s += " AND (" + oDateEnd.GetSQLCondition2("TP.Дата") + ")";
				s+=")";
	
			}
			return s;
        }
    }
}
