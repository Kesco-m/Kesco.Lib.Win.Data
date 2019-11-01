using System.Xml;
using Kesco.Lib.Win.Data.Options;
using Kesco.Lib.Win.Data.Types;

namespace Kesco.Lib.Win.Data.Business.Documents
{
    public abstract class DocumentTypeOption : Option
    {
        private DocumentType type;

        #region ACCESSORS

        public DocumentType Type
        {
            get { return type; }
            set
            {
                if (type == value) return;
                type = value;
            }
        }

        public byte Filter { get; set; }

        public string TypeName
        {
            get
            {
                if (Type == null) return "";
                if (Type.IsUnavailable) return "#" + Type.ID;
                return Type.Name;
            }
        }

        #endregion

        public DocumentTypeOption(string name) : base(name)
        {
        }


        #region XML

        public override void SaveToXmlElement(XmlElement el)
        {
            base.SaveToXmlElement(el);
            if (Type == null)
            {
                el.SetAttribute("TypeID", "");
                el.SetAttribute("TypeName", "");
                el.SetAttribute("Filter", "0");
            }
            else
            {
                el.SetAttribute("TypeID", Type.ID.ToString());
                el.SetAttribute("TypeName", TypeName);
                el.SetAttribute("Filter", Filter.ToString());
            }
        }

        public override void LoadFromXmlElement(XmlElement el)
        {
            base.LoadFromXmlElement(el);
            int id = KInt.FromXmlString(el.GetAttribute("TypeID"));
            try
            {
                Filter = byte.Parse(el.GetAttribute("Filter"));
            }
            catch
            {
                Filter = 0;
            }
            type = id == int.MinValue ? null : new DocumentType(id);
        }

        #endregion
    }
}
