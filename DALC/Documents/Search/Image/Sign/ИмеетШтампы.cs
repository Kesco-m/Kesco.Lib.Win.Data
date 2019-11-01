using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.Sign
{
	/// <summary>
	/// Summary description for ИмеетШтампы.
	/// </summary>

	[Option("Image.Sing.ИмеетШтампы", typeof(ИмеетШтампы))]
	public class ИмеетШтампы:ListOption
	{
		protected ИмеетШтампы(XmlElement el)
			: base(el)
		{
		
			emptyValueText=Resources.GetString("emptyValueText");
			
			shortTextPrefix=Resources.GetString("shortTextPrefix");
			shortTextPostfix="";

			htmlPrefix=Resources.GetString("htmlPrefix");
			htmlPrefix2=Resources.GetString("htmlPrefix2");
			htmlPostfix="";
			
			textItemPrefix="[";
			textItemPostfix="]";
		}


		public override bool OpenWindow()
		{
			return false;
		}

		

		public  override string GetSQL( bool throwOnError)
		{
			//return null;
			try
			{
				string[] vals= GetValues(throwOnError);
				if (vals.Length!=0)
					return GetSQLCondition(@"
EXISTS (SELECT *
FROM Документы.dbo.ПодписиДокументов TI WITH(NOLOCK)
WHERE TI.КодДокумента=T0.КодДокумента AND TI.ТипПодписи = 100 AND
(TI.КодШтампа = @VAL))"
						);
				else
					return @"
EXISTS (SELECT *
FROM Документы.dbo.ПодписиДокументов TI WITH(NOLOCK)
WHERE TI.КодДокумента=T0.КодДокумента AND TI.ТипПодписи = 100)";

			}
			catch(Exception ex)
			{
				if(throwOnError) throw ex;
				return null;
			}
		}

		public override string GetText()
		{
			string[] vals= GetValues(false);
			if (vals.Length==0) return htmlPrefix+emptyValueText+htmlPostfix;
			return base.GetText ();
		}
		public override string GetShortText()
		{
			string[] vals= GetValues(false);
			if (vals.Length==0) return shortTextPrefix+emptyValueText+shortTextPostfix;
			return base.GetText ();
		}

		public override string GetHTML()
		{
			string[] values = GetValues(false);
			string s = "";
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ListOption));
			if (values.Length == 0) s = emptyValueText;
			else for (int i = 0; i < values.Length; i++)
					s += (s.Length > 0 ? (Mode == Modes.AND ? resources.GetString("And") : resources.GetString("Or")) : "") +
						GetItemText(values[i]);

			return System.Text.RegularExpressions.Regex.Replace((GetValues(false).Length > 1 ? htmlPrefix2 : htmlPrefix), "[ ]$", "");// +
			//    s +
			//    htmlPostfix;
		}
	}
}
