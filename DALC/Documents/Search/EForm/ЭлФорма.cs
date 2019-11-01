using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm
{
	[Option("Ёл‘орма", typeof(Ёл‘орма))]
	public class Ёл‘орма : Option
	{
		protected Ёл‘орма(XmlElement el) : base(el)
		{
			NegativeOption = new[] { "Ёл‘ормаќтсутствует" };
			shortTextPrefix = Resources.GetString("shortTextPrefix");
		}

		public override string GetSQL(bool throwOnError)
		{
			return
				@"
                EXISTS (SELECT *
                FROM ƒокументы.dbo.vwƒокументыƒанные TI WITH(NOLOCK)
                WHERE TI. одƒокумента=T0. одƒокумента)
                ";
		}
	}
}