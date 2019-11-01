using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Transaction
{
    [Option("Transaction.���������������", typeof (���������������))]
    public class ��������������� : DateOption
    {
        protected ���������������(XmlElement el) : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            htmlPrefix = Resources.GetString("htmlPrefix");
            shortTextPostfix = htmlPostfix = "";
        }

        public override string GetSQL(bool throwOnError)
        {
            string sql;
            try
            {
                sql =
                    @"
                    EXISTS (SELECT *
                    FROM dbo.vw���������� T1 WITH(NOLOCK)
                    WHERE T1.��������������������� = T0.������������ AND " +
                    GetSQLCondition("T1.����") + ")" +
                    @" OR EXISTS (SELECT *
                    FROM dbo.vw���������� T1 WITH(NOLOCK)
                    WHERE T1.������������������������� = T0.������������ AND " +
                    GetSQLCondition("T1.����") + ")";

            }
            catch (Exception ex)
            {
                if (throwOnError)
                    throw ex;
                return null;
            }
            return sql;
        }
    }
}
