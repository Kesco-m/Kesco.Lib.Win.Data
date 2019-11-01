using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm.Sign
{
    [Option("EForm.Sing.��������������", typeof (��������������))]
    public class �������������� : DateOption
    {
        protected ��������������(XmlElement el) : base(el)
        {
            shortTextPrefix = htmlPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = htmlPostfix = "";
        }

        public override string GetSQL(bool throwOnError)
        {
            string sql;
            try
            {
                sql =
                    @"
EXISTS (SELECT     *
FROM	����������������� TI WITH(NOLOCK)
WHERE TI.������������=T0.������������ AND TI.����������������������� IS NULL AND " +
                    GetSQLCondition("TI.����") + ")";

            }
            catch (Exception ex)
            {
                if (throwOnError) throw ex;
                return null;
            }
            return sql;
        }
    }
}
