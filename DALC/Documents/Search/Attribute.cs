using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search
{
	[Localizable(true)]
	public class OptionAttribute : Attribute
	{
		public OptionAttribute(string name, string description)
		{
			Name = name;
			Description = description;
		}

		public OptionAttribute(string name, Type type)
		{
			Name = name;
			var res = new ResourceManager(type);
			try
			{
				string des = res.GetString("Meta.Description");
				if(!string.IsNullOrEmpty(des))
					Description = des;
			}
			catch
			{
				Description = name;
			}
		}

		public OptionAttribute(string name, Type type, Type mainOption, int index) : this(name, type)
		{
			MainOption = mainOption;
			Index = index;
		}

		public string Name { get; private set; }

		public string Description { get; set; }

		public Type MainOption { get; private set; }

		public int Index { get; private set; }
	}

	public class SeparateOptionAttribute : Attribute
	{
		public string Name { get; private set; }
		public Type SeparateType { get; private set; }
		public SeparateOptionAttribute(string name, Type separateType)
		{
			Name = name;
			SeparateType = separateType;
		}
	}
}
