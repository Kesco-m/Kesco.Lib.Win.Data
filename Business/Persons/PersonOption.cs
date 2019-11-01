using System.Xml;
using Kesco.Lib.Win.Data.Options;
using Kesco.Lib.Win.Data.Types;

namespace Kesco.Lib.Win.Data.Business.Persons
{
	public abstract class PersonOption:Option
	{
		Person person;
		
		#region ACCESSORS
		public Person @Person
		{
			get
			{
				return person;
			}
			set
			{
				if (person==value) return;
				person=value;
			}
		}
		public string PersonName
		{
			get
			{
				if (@Person==null) return "";
				if (@Person.IsUnavailable) return "#"+@Person.ID;
				return @Person.ShortName;
			}
		}

		#endregion
		
		public PersonOption(string name):base(name)
		{
		}

		#region XML
		public override void SaveToXmlElement(XmlElement el)
		{
			base.SaveToXmlElement(el);
			if(@Person==null)
			{
				el.SetAttribute("PersonID","");
				el.SetAttribute("PeresonName","");
			}
			else
			{
				el.SetAttribute("PersonID",@Person.ID.ToString());
				el.SetAttribute("PersonName",PersonName);
			}
		}
		public override void LoadFromXmlElement(XmlElement el)
		{
			base.LoadFromXmlElement(el);
			int id = KInt.FromXmlString(el.GetAttribute("PersonID"));
			person = id==int.MinValue?null:new Person(id);
		}


		#endregion
	}
}
