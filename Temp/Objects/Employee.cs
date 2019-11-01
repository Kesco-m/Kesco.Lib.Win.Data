using System;
using Kesco.Lib.Win.Data.DALC.Corporate;

namespace Kesco.Lib.Win.Data.Temp.Objects
{
    /// <summary>
    /// Summary description for Employee.
    /// </summary>
    public class Employee : IDObject
    {
        private string shortRusName;
        private string longRusName;
        private string shortEngName;
        private string longEngName;
        private string currentLanguage;
        private int workState = -1;

        private EmployeeDALC empData;

        public Employee(int id, EmployeeDALC empData) : base(id)
        {
            this.empData = empData;
        }

        public Employee(int id, string shortName, string longName, EmployeeDALC empData) : this(id, empData)
        {
            shortRusName = shortName;
            longRusName = longName;
        }

        public Employee(int id, string shortName, string longName, string language, EmployeeDALC empData)
            : this(id, shortName, longName, empData)
        {
            currentLanguage = language;
        }

        public Employee(int id, string shortRusName, string longRusName, string shortEngName, string longEngName,
                        EmployeeDALC empData) : this(id, shortRusName, longRusName, empData)
        {
            this.shortEngName = shortEngName;
            this.longEngName = longEngName;
        }

        public Employee(int id, string shortRusName, string longRusName, string shortEngName, string longEngName,
                        string language, EmployeeDALC empData)
            : this(id, shortRusName, longRusName, shortEngName, longEngName, empData)
        {
            currentLanguage = language;
        }

        #region Accessors		

        public string ShortName
        {
            get { return shortRusName ?? (shortRusName = empData.GetEmployee(id, false)); }
        }

        public string LongName
        {
            get { return longRusName ?? (longRusName = empData.GetEmployee(id, true)); }
        }

        public string ShortEngName
        {
            get { return shortEngName ?? (shortEngName = empData.GetEmployeeEng(id, false)); }
        }

        public string LongEngName
        {
            get { return longEngName ?? (longEngName = empData.GetEmployeeEng(id, true)); }
        }

        public string Language
        {
            get { return currentLanguage ?? (currentLanguage = empData.GetEmployeeLanguage(id)); }
        }

        public int WorkState
        {
            get
            {
                if (workState == -1)
                {
                    object obj = empData.GetField(empData.StatusField, id);
                    if (!obj.Equals(DBNull.Value))
                        workState = (byte) obj;
                }
                return workState;
            }
        }

        public string Name
        {
            get
            {
                if (Language.StartsWith("ru"))
                {
                    if (longRusName != null)
                        return longRusName;
                    if (shortRusName != null)
                        return shortRusName;
                    return LongName;
                }

                if (longEngName != null)
                    return longEngName;
                if (shortEngName != null)
                    return shortEngName;
                return LongName;
            }
        }

        public string EmpEMail
        {
            get { return empData.GetEmpEMail(id); }
        }

        #endregion

        public static Employee GetSystemEmployee(EmployeeDALC empData)
        {
            return new Employee(empData.GetSystemEmployeeID(), empData);
        }

        public override bool Equals(object obj)
        {
            if (obj is Employee)
            {
                var emp = obj as Employee;
                return (id == emp.ID);
            }

            return false;
        }
    }
}