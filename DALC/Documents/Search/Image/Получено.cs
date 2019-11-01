using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image
{
	[Option("Image.Получено", typeof(Получено))]
	public class Получено : DateOption
	{
		protected Получено(XmlElement el) : base(el)
		{
			shortTextPrefix = Resources.GetString("shortTextPrefix");
			shortTextPostfix = "";

			htmlPrefix = Resources.GetString("htmlPrefix");
			htmlPostfix = "";
		}


		public override string GetSQL(bool throwOnError)
		{
			return
				@"EXISTS (SELECT *
                FROM dbo.vwИзображенияДокументовlog T1 (NOLOCK)
                    WHERE T1.КодДокумента = T0.КодДокумента AND T1.Direction = 1)";
		}
	}
}