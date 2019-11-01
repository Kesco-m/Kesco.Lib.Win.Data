using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm
{
    [Option("EForm.ChangedBy", typeof (ChangedBy))]
    public class ChangedBy : EmployeeListOption
    {

        protected ChangedBy(XmlElement el) : base(el)
        {
            Mode = Modes.OR;
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPrefix2 = Resources.GetString("htmlPrefix2");
            htmlPostfix = "";

            textItemPrefix = "[";
            textItemPostfix = "]";
        }

        public override string GetSQL(bool throwOnError)
        {
            try
            {
                string[] values = GetValues(throwOnError);
                if (values.Length == 0) throw new Exception(Resources.GetString("GetSQL") + Meta.Description);

                string s = GetSQLCondition("TI.Изменил=@VAL");
                if (s.Length > 0) s = " AND (" + s + ")";

                return
                    @"
                    EXISTS (SELECT *
                    FROM Документы.dbo.vwДокументыДанные TI WITH(NOLOCK)
                    WHERE TI.КодДокумента=T0.КодДокумента" +
                    s + @")
                    ";
            }
            catch (Exception ex)
            {
                if (throwOnError)
                    throw ex;
                return null;
            }
        }
    }
}
