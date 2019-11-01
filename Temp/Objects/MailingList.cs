using System;
using System.Collections.Generic;
using Kesco.Lib.Win.Data.DALC.Documents;

namespace Kesco.Lib.Win.Data.Temp.Objects
{
    /// <summary>
    /// Summary description for MailingList.
    /// </summary>
    public class MailingListItem : IDObject
    {
        private string name;
        private string author;
        private bool editable = true;
        private Employee employee;
        private List<Employee> employees;
        private List<Employee> sharedEmploees;

        private MailingListDALC mailingListData;

        internal MailingListItem(int id, MailingListDALC mailingListData) : base(id)
        {
            this.mailingListData = mailingListData;
        }

        internal MailingListItem(int id, string name, MailingListDALC mailingListData) : this(id, mailingListData)
        {
            this.name = name;
        }

        internal MailingListItem(int id, string name, MailingListDALC mailingListData, string author)
            : this(id, name, mailingListData)
        {
            this.author = author;
        }

        internal MailingListItem(int id, string name, Employee employee, MailingListDALC mailingListData)
            : this(id, name, mailingListData)
        {
            this.employee = employee;
        }

        internal MailingListItem(int id, string name, Employee employee, MailingListDALC mailingListData, string author)
            : this(id, name, employee, mailingListData)
        {
            this.author = author;
        }

        internal MailingListItem(int id, string name, List<Employee> employees, MailingListDALC mailingListData)
            : this(id, name, mailingListData)
        {
            this.employees = employees;
        }

        #region Accessors

        public string Name
        {
            get { return name ?? (name = (string) mailingListData.GetField(mailingListData.NameField, id)); }
            set { name = value; }
        }

        public string Author
        {
            get { return author ?? string.Empty; }
            set { author = value; }
        }

        public bool Editable
        {
            get { return editable; }
            set { editable = value; }
        }

        public Employee Employee
        {
            get
            {
                if (employee == null)
                {
                    try
                    {
                        employee = mailingListData.GetMailingListEmp(id);
                    }
                    catch (Exception ex)
                    {
                        throw ex; // передаем дальше...
                    }
                }

                return employee;
            }
            set { employee = value; }
        }

        public List<Employee> Employees
        {
            get
            {
                if (employees == null)
                {
                    try
                    {
                        employees = mailingListData.GetMailingListEmps(id);
                    }
                    catch (Exception ex)
                    {
                        throw ex; // передаем дальше...
                    }
                }

                return employees;
            }
            set { employees = value; }
        }

        public List<Employee> SharedEmploees
        {
            get { return sharedEmploees ?? (sharedEmploees = mailingListData.GetMailingListSharedEmps(id)); }
            set { sharedEmploees = value; }
        }

        #endregion

        public override bool Equals(object obj)
        {
            var ml = obj as MailingListItem;
            return ml != null && id == ml.ID;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override string ToString()
        {
            return name != null
                       ? name + (string.IsNullOrEmpty(author) ? string.Empty : string.Format(" ({0})", author))
                       : "Null";
        }

        public bool Delete()
        {
            return mailingListData.Delete(id);
        }
    }
}