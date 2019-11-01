using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image
{
	[Option("Image.Сохранил", typeof(Сохранил))]
	public class Сохранил : EmployeeListOption
	{
		protected Сохранил(XmlElement el) : base(el)
		{
			Mode = Modes.OR;
			shortTextPrefix = Resources.GetString("shortTextPrefix");
			shortTextPostfix = "";

			htmlPrefix = Resources.GetString("htmlPrefix");
			htmlPrefix2 = Resources.GetString("htmlPrefix2");
			htmlPostfix = "";

			textItemPrefix = "[";
			textItemPostfix = "]";
		}

		public override string GetSQL(bool throwOnError)
		{
			try
			{
				string[] values = GetValues(throwOnError);
				if(values.Length == 0)
					throw new Exception(Resources.GetString("GetSQL") + Meta.Description);

				return null;
			}
			catch(Exception ex)
			{
				if(throwOnError)
					throw ex;
				return null;
			}
		}
	}
}