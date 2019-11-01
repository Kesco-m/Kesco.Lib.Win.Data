using System.Text.RegularExpressions;
using System.Xml;
using Kesco.Lib.Win.Data.Business.Persons;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns
{
    public class PersonOption : ValueOption
    {
        public PersonOption(XmlElement el) : base(el)
        {
        }

        public override string GetItemText(string key)
        {
            if (!Regex.IsMatch(key, "^\\d+$")) return "#" + key;
            var p = new Person(int.Parse(key));
            if (p.IsUnavailable) return "#" + key;
            return p.ShortName;
        }
    }
}
