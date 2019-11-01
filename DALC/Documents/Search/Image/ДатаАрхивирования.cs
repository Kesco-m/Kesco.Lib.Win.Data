using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image
{
	/// <summary>
	/// Summary description for �����������������.
	/// </summary>
	[Option("Image.�����������������", typeof(�����������������))]
	public class ����������������� : DateOption
	{
		protected �����������������(XmlElement el) : base(el)
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
