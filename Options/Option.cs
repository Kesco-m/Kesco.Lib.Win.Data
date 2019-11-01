using System.Xml;
using Kesco.Lib.Win.Data.Types;

namespace Kesco.Lib.Win.Data.Options
{
    /// <summary>
    /// Summary description for Option.
    /// </summary>
    public abstract class Option
    {
        private bool enabled;
        private bool @fixed;

        /// <summary>
        /// ��� ����� (������ ���� ��������� � ������ ����� ���������)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ���������� ����� �� ������ ����� �������������� ��� ������
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled == value) return;
                enabled = value;
            }
        }

        /// <summary>
        /// ����������, ����� �� ������ ����� ���������� �������������
        /// </summary>
        public bool Fixed
        {
            get { return @fixed; }
            set
            {
                if (@fixed == value) return;
                @fixed = value;
            }
        }

        public virtual bool Validate(bool throwOnError)
        {
            return true;
        }

        /// <summary>
        /// ���������� HTML �����������, ������� ��������� ����� ������
        /// </summary>
        public abstract string GetHtml();

        //HTML � �������� ���������� HTMLView
        public abstract string GetText(); //�������� ����� ������
        public abstract string GetShortText(); //��� ������������ ��������
        public abstract string GetSQL(); //
        public abstract string GetCaption(); //

        public Option(string name)
        {
            Name = name;
            Init();
        }

        public virtual void Init()
        {

        }

        #region XML

        public virtual void SaveToXmlElement(XmlElement el)
        {
            el.SetAttribute("Name", Name);
            el.SetAttribute("Enabled", KBoolean.ToXmlString(Enabled));
            el.SetAttribute("Fixed", KBoolean.ToXmlString(Fixed));
        }

        public virtual void LoadFromXmlElement(XmlElement el)
        {
            enabled = KBoolean.FromXmlString(el.GetAttribute("Enabled"), false);
            @fixed = KBoolean.FromXmlString(el.GetAttribute("Fixed"), false);
        }

        #endregion
    }
}
