using System;

namespace Kesco.Lib.Win.Data.DALC.Documents.New_Search.Base
{
	public class NumericTypeAttribute : Attribute
	{
		public NumericTypeAttribute(string name, Type type)
		{
			Name = name;
			SearchType = type;
		}

		public string Name { get; private set; }
		public Type SearchType { get; private set; }
	}
}