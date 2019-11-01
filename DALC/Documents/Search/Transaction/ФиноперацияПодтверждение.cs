using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Transaction
{
    /// <summary>
    /// Summary description for ������������������������.
    /// </summary>

    [Option("Transaction.�������������", typeof (������������������������))]
    public class ������������������������ : Option
    {
        protected ������������������������(XmlElement el) : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"
                EXISTS (SELECT *
                FROM dbo.vw���������� T1 WITH(NOLOCK)
                WHERE (T1.������������������������� = T0.������������))
                ";
        }
    }
}
