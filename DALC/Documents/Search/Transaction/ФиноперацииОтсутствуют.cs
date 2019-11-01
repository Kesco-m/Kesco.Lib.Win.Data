using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Transaction
{
    /// <summary>
    /// Summary description for ����������������������.
    /// </summary>
    [Option("Transaction.Hasnot", typeof (����������������������))]
    public class ���������������������� : Option
    {
        protected ����������������������(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"Transaction.Has"};

            shortTextPrefix = "";
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"
                NOT EXISTS (SELECT *
                FROM dbo.vw���������� T1 WITH(NOLOCK)
                WHERE T1.��������������������� = T0.������������)
                AND NOT EXISTS (SELECT *
                FROM dbo.vw���������� T1 WITH(NOLOCK)
                WHERE T1.������������������������� = T0.������������)
                ";
        }
    }
}