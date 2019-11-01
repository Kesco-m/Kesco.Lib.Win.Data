using System.Xml;

namespace Kesco.Lib.Win.Data.Options
{
    public abstract class SimpleTextOption : Option
    {
        private string text;

        public string Text
        {
            get { return text; }
            set
            {
                if (value == null) value = "";
                if (text.Equals(value)) return;
                text = value;
            }
        }

        public SimpleTextOption(string name) : base(name)
        {
            text = "";
        }


        #region Xml

        public override void SaveToXmlElement(XmlElement el)
        {
            base.SaveToXmlElement(el);
            el.SetAttribute("Text", Text);
        }

        public override void LoadFromXmlElement(XmlElement el)
        {
            base.LoadFromXmlElement(el);
            text = el.GetAttribute("Text");
        }

        #endregion

    }
}
