using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.Sign
{
	/// <summary>
	/// опция поиска по наличию штампа "ДСП".
	/// </summary>
	[Option("Image.Sing.УстановленДСП", typeof(УстановленДСП))]
	public class УстановленДСП : EmployeeListOption
	{
		protected УстановленДСП(XmlElement el) : base(el)
		{
			NegativeValueOption = new string[] { "Image.NoSing.НеУстановленДСП" };
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
		FROM Документы.dbo.ПодписиДокументов TI
		WHERE TI.КодДокумента=T0.КодДокумента
		AND TI.ТипПодписи=101
		AND (TI.КодСотрудника = @VAL))");
				else
					return @"
		EXISTS (SELECT *
		FROM Документы.dbo.ПодписиДокументов TI
		WHERE TI.КодДокумента=T0.КодДокумента 
		AND TI.ТипПодписи=101)";
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