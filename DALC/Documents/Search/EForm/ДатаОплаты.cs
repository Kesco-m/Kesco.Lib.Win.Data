using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm
{
    [Option("EForm.����������", typeof (����������))]
    public class ���������� : DateOption
    {
        protected ����������(XmlElement el)
            : base(el)
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
EXISTS (SELECT * FROM vw������������������������ TI WITH(NOLOCK) WHERE TI.���������������� IN (SELECT ���������������� FROM dbo.��������������  WITH(NOLOCK)
WHERE ������������� = '���� ������' AND �������������� = '����2') AND TI.������������=T0.������������ AND " +
                    GetSQLCondition("TI.����2") + ")";

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
