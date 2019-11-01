using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.Sign
{
	/// <summary>
	/// Summary description for ��������.
	/// </summary>
	[Option("Image.Sing.��������", typeof(��������))]
	public class �������� : EmployeeListOption
	{
		protected ��������(XmlElement el) : base(el)
		{
			NegativeValueOption = new String[] { "Image.NoSing.����������", "Image.NoSing.��������������" };
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
FROM ���������.dbo.����������������� TI WITH(NOLOCK)
WHERE TI.������������=T0.������������ AND TI.����������������������� IS NOT NULL AND TI.����������<>101 AND
(TI.��������������� = @VAL OR TI.������������� = @VAL))");
				else
					return @"
EXISTS (SELECT *
FROM ���������.dbo.����������������� TI WITH(NOLOCK)
WHERE TI.������������=T0.������������ AND TI.����������������������� IS NOT NULL AND TI.����������<>101)";
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