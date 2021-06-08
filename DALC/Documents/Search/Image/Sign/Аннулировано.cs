using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.New_Search.Base;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.Sign
{
	/// <summary>
	/// Summary description for ������������.
	/// </summary>
	[AllAny("������������", true)]
	[Option("Image.Sing.������������", typeof(������������))]
	[SeparateOption("Image.Sing.������������", typeof(�����������))]
	public class ������������ : EmployeeListOption
	{
		public bool IsSingle;
		protected ������������(XmlElement el) : base(el)
		{
			NegativeOption = new string[] { "Image.NoSing.�������������" };
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
			return null;
			/*			try
						{
							string[] vals = GetValues(throwOnError);

							if(vals.Length!=0)
								return GetSQLCondition(@"
			EXISTS (SELECT *
			FROM ���������.dbo.����������������� TI
			WHERE TI.������������=T0.������������
			AND TI.����������=1
			AND (TI.��������������� = @VAL OR TI.������������� = @VAL))"
									);
							else
								return @"
			EXISTS (SELECT *
			FROM ���������.dbo.����������������� TI
			WHERE TI.������������=T0.������������ 
			AND TI.����������=1
			)";
				
						}
						catch(Exception ex)
						{
							if(throwOnError) throw ex;
							return null;
						}
						*/
		}

		public override string GetText()
		{
			string[] vals = GetValues(false);
			if (vals.Length == 0) return htmlPrefix + emptyValueText + htmlPostfix;
			return base.GetText();
		}

		//override 

		public override string GetShortText()
		{
			string[] vals = GetValues(false);
			if (vals.Length == 0) return shortTextPrefix + emptyValueText + shortTextPostfix;
			return base.GetText();
		}
	}
}