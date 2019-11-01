using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image
{
	/// <summary>
	/// Summary description for ДатаСохранения.
	/// </summary>
	[Option("Image.ИзмененоХранилище", typeof(ИзмененоХранилище), typeof(Хранилище),2)]
	public class ИзмененоХранилище : DateOption
	{
		protected ИзмененоХранилище(XmlElement el) : base(el)
		{
			emptyValueText = Resources.GetString("emptyValueText");
			shortTextPrefix = htmlPrefix = Resources.GetString("shortTextPrefix");
			shortTextPostfix = htmlPostfix = "";
		}

		public override string GetSQL(bool throwOnError)
		{
			return null;
		}
	}
}