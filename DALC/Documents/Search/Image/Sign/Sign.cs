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
        private ������������ oEnd;
        private ����������������� oDateEnd;

        protected Sign(XmlElement el) : base(el)
        {
            XmlElement el0;
            el0 = (XmlElement) el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.Sing.������������']");
            if (el0 != null) oEnd = (������������) CreateOption(el0);

            el0 = (XmlElement) el.OwnerDocument.SelectSingleNode("Options/Option[@name='Image.Sing.�����������������']");
            if (el0 != null) oDateEnd = (�����������������) CreateOption(el0);
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
FROM ���������.dbo.����������������� TI WITH(NOLOCK)
WHERE TI.������������=T0.������������ AND TI.����������������������� IS NOT NULL ";

			if(oEnd != null || oDateEnd != null)
			{
				s += " AND (TI.����������=2";
				try
				{
					if(oEnd.GetValues(false).Length > 0)
						s += " AND (" + oEnd.GetSQLCondition2("TI.��������������� = @VAL OR TI.������������� = @VAL") +  ")";
				}
				catch(Exception ex)
				{
					Env.WriteToLog(ex);
				}
				if(oDateEnd != null)
					s += " AND (" + oDateEnd.GetSQLCondition2("TI.����") + ")";

				s += ")\n";
			}
			s += ")\n";
			if((oEnd != null && !oEnd.IsSeparate())|| (oDateEnd != null && !oDateEnd.IsSeparate()))
			{
				s+=@"OR EXISTS(SELECT * FROM ���������.dbo.����������������� TP WITH(NOLOCK)
		WHERE TP.������������=T0.������������ AND TP.����������=1 AND TP.����������������������� IS NULL";
				try
				{
					if(oEnd.GetValues(false).Length > 0)
						s += " AND (" + oEnd.GetSQLCondition2("TP.��������������� = @VAL OR TP.������������� = @VAL") +  ")";
				}
				catch(Exception ex)
				{
					Env.WriteToLog(ex);
				}
				if(oDateEnd != null)
					s += " AND (" + oDateEnd.GetSQLCondition2("TP.����") + ")";
				s+=")";
	
			}
			return s;
        }
    }
}
