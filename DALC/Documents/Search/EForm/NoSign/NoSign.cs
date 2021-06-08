using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm.NoSign
{
	/// <summary>
	/// Дополнительный класс для поиска в изображениях
	/// </summary>
	[Option("EForm.NoSign", ".")]
	class NoSign : Option
	{
		protected NoSign(XmlElement el) : base(el)
		{
		}

		public override string GetHTML()
		{
			return string.Empty;
		}

		public override string GetSQL(bool throwOnError)
		{
			return @"EXISTS (SELECT * FROM vwДокументыДанные WHERE КодДокумента=T0.КодДокумента)";
		}
	}
}
