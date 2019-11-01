using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Transaction
{
    /// <summary>
    /// Summary description for Transacton_Has.
    /// </summary>
    [Option("Transaction.Has", typeof (�����������))]
    public class ����������� : Option
    {
        protected �����������(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"Transaction.Hasnot"};
            shortTextPrefix = Resources.GetString("shortTextPrefix");
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"
                EXISTS (SELECT *
                FROM dbo.vw���������� T1 WITH(NOLOCK)
                WHERE T1.��������������������� = T0.������������)
                OR EXISTS (SELECT *
                FROM dbo.vw���������� T1 WITH(NOLOCK)
                WHERE T1.������������������������� = T0.������������)
                ";
        }
    }
}