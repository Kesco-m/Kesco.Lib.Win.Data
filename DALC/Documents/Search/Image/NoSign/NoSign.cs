using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.NoSign
{
	/// <summary>
	/// Дополнительный класс для поиска в изображениях
	/// </summary>
	[Option("Image.NoSign", ".")]
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
			return @"T0.КодИзображенияДокументаОсновного IS NOT NULL";
		}
	}
}
