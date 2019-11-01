using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kesco.Lib.Win.Data.DALC.Documents.New_Search.Base
{
	/// <summary>
	/// атрибут для изменения типа поиска по спискам
	/// </summary>
	class AllAnyAttribute : Attribute
	{
		/// <summary>
		/// атрибут для изменения типа поиска по спискам
		/// </summary>
		/// <param name="name">имя атрибута</param>
		/// <param name="type">значение атрибута</param>
		public AllAnyAttribute(string name, Boolean type)
		{
			Name = name;
			SearchType = type;
		}

		public string Name { get; private set; }
		public Boolean SearchType { get; private set; }
	}
}
