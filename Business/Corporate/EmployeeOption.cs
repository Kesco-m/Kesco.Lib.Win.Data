using System.Xml;
using Kesco.Lib.Win.Data.Options;
using Kesco.Lib.Win.Data.Types;

namespace Kesco.Lib.Win.Data.Business.Corporate
{
    public abstract class EmployeeOption : Option
    {
        private Employee employee;

        #region ACCESSORS

        public Employee @Employee
        {
            get { return employee; }
            set
            {
                if (employee == value) return;
                employee = value;
            }
        }

        public string EmployeeName
        {
            get
            {
                if (@Employee == null) return "";
                if (@Employee.IsUnavailable) return "#" + @Employee.ID;
                return @Employee.Name;
            }
        }

        #endregion

        public EmployeeOption(string name)
            : base(name)
        {
        }

        #region XML

        public override void SaveToXmlElement(XmlElement el)
        {
            base.SaveToXmlElement(el);
            if (Employee == null)
            {
                el.SetAttribute("EmployeeID", "");
                el.SetAttribute("EmployeeName", "");
            }
            else
            {
                el.SetAttribute("EmployeeID", Employee.ID.ToString());
                el.SetAttribute("EmployeeName", EmployeeName);
            }
        }

        public override void LoadFromXmlElement(XmlElement el)
        {
            base.LoadFromXmlElement(el);
            int id = KInt.FromXmlString(el.GetAttribute("EmployeeID"));
            employee = id == int.MinValue ? null : new Employee(id);
        }

        #endregion
    }
}
