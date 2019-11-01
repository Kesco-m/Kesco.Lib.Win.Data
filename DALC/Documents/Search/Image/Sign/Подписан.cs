using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.Sign
{
	/// <summary>
	/// Summary description for Подписан.
	/// </summary>
	[Option("Image.Sing.Подписан", typeof(Подписан))]
	public class Подписан : EmployeeListOption
	{
		protected Подписан(XmlElement el) : base(el)
		{
			NegativeValueOption = new String[] { "Image.NoSing.НеПодписан", "Image.NoSing.НеПодписанМной" };
			emptyValueText = Resources.GetString("emptyValueText");

			shortTextPrefix = Resources.GetString("shortTextPrefix");
			shortTextPostfix = "";

			htmlPrefix = Resources.GetString("htmlPrefix");
			htmlPrefix2 = Resources.GetString("htmlPrefix2");
			htmlPostfix = "";

			textItemPrefix = "[";
			textItemPostfix = "]";
		}

		public override bool OpenWindow()
		{
			return false;
		}

		public override string GetSQL(bool throwOnError)
		{
			try
			{
				string[] vals = GetValues(throwOnError);
				if(vals.Length != 0)
					return GetSQLCondition(@"
EXISTS (SELECT *
FROM Документы.dbo.ПодписиДокументов TI WITH(NOLOCK)
WHERE TI.КодДокумента=T0.КодДокумента AND TI.КодИзображенияДокумента IS NOT NULL AND TI.ТипПодписи<>101 AND
(TI.КодСотрудникаЗа = @VAL OR TI.КодСотрудника = @VAL))");
				else
					return @"
EXISTS (SELECT *
FROM Документы.dbo.ПодписиДокументов TI WITH(NOLOCK)
WHERE TI.КодДокумента=T0.КодДокумента AND TI.КодИзображенияДокумента IS NOT NULL AND TI.ТипПодписи<>101)";
			}
			catch(Exception ex)
			{
				if(throwOnError)
					throw ex;
				return null;
			}
		}

		public override string GetText()
		{
			string[] vals = GetValues(false);
			if(vals.Length == 0)
				return htmlPrefix + emptyValueText + htmlPostfix;
			return base.GetText();
		}

		public override string GetShortText()
		{
			string[] vals = GetValues(false);
			if(vals.Length == 0)
				return shortTextPrefix + emptyValueText + shortTextPostfix;
			return base.GetText();
		}
	}
}