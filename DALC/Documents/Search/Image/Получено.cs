using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image
{
	[Option("Image.��������", typeof(��������))]
	public class �������� : DateOption
	{
		protected ��������(XmlElement el) : base(el)
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
                FROM dbo.vw���������������������log T1 (NOLOCK)
                    WHERE T1.������������ = T0.������������ AND T1.Direction = 1)";
		}
	}
}