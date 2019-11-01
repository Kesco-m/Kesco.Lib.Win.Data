using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image
{
	/// <summary>
	/// Summary description for ДатаАрхивирования.
	/// </summary>
	[Option("Image.ДатаАрхивирования", typeof(ДатаАрхивирования))]
	public class ДатаАрхивирования : DateOption
	{
		protected ДатаАрхивирования(XmlElement el) : base(el)
		{
			shortTextPrefix = htmlPrefix = Resources.GetString("shortTextPrefix");
			shortTextPostfix = htmlPostfix = "";
		}

		public override string GetSQL(bool throwOnError)
		{
			if(throwOnError && (Mode == Modes.None))
				throw new Exception(Resources.GetString("GetSQL"));
			return null;
		}
	}
}
