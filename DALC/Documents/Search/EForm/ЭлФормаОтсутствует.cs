using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm
{
    [Option("Ёл‘ормаќтсутствует", typeof (Ёл‘ормаќтсутствует))]
    public class Ёл‘ормаќтсутствует : Option
    {
        protected Ёл‘ормаќтсутствует(XmlElement el)
            : base(el)
        {
            NegativeOption = new[] {"Ёл‘орма", "ѕечатна€‘орма"};
            shortTextPrefix = Resources.GetString("shortTextPrefix");
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"
                NOT EXISTS (SELECT *
                FROM ƒокументы.dbo.vwƒокументыƒанные TI WITH(NOLOCK)
                WHERE TI. одƒокумента=T0. одƒокумента)
                ";
        }
    }
}