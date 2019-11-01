using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Xml;
using Kesco.Lib.Win.Data.Business.Persons;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns
{
    public class PersonListOption : ListOption
    {

        private Modes _mode;

        [Flags]
        public new enum Modes
        {
            OR = 1, //default
            AND = 2, //ТипЛица
            PersonOrganization = 4,
            PersonPhysical = 8,
            PersonBank = 16, //Проверенный бухгалтерией
            PersonCheck = 32,
            PersonUncheck = 64, //Cтрана регистрации
            PersonArea = 128, //Ответственные сотрудники
            PersonUsers = 256, //Организационно-правовой форме
            PersonOPForma = 512, //Ограничения по типам лиц
            PersonThemes = 1024,
            PersonSubThemes = 2048, //Ограничения по бизнас проектам
            PersonBusinessProject = 4096,
            PersonSubBusinessProject = 8192,
            PersonAnyBusinessProject = 16384,
            PersonNullBusinessProject = 32768

        }

        private int iDBusinessProject = -1;

        public int IDBusinessProject
        {
            get { return iDBusinessProject; }
            set { iDBusinessProject = value; }
        }

        private string nameBusinessProject = "";

        public string NameBusinessProject
        {
            get { return nameBusinessProject; }
            set { nameBusinessProject = value; }
        }

        private int iDArea = -1;

        public int IDArea
        {
            get { return iDArea; }
            set { iDArea = value; }
        }

        private string nameArea = "";

        public string NameArea
        {
            get { return nameArea; }
            set { nameArea = value; }
        }

        private int iDOPForm = -1;

        public int IDOPForm
        {
            get { return iDOPForm; }
            set { iDOPForm = value; }
        }

        private string nameOPForm = "";

        public string NameOPForm
        {
            get { return nameOPForm; }
            set { nameOPForm = value; }
        }

        private Hashtable listUsers = new Hashtable(); //ответственные сотрудники

        public Hashtable ListUsers
        {
            get { return listUsers; }
            set { listUsers = value; }
        }

        private Hashtable listTypes = new Hashtable(); //типы

        public Hashtable ListTypes
        {
            get { return listTypes; }
            set { listTypes = value; }
        }

        public new Modes Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;

                el.SetAttribute("mode", _mode == Modes.AND ? "and" : "or");
            }
        }

        public void AddAttribute(string name, string val)
        {
            el.SetAttribute(name, val);
        }

        public void RemoveAttribute(string name)
        {
            el.RemoveAttribute(name);
        }

        public PersonListOption(XmlElement el)
            : base(el)
        {
            if (el.GetAttribute("mode").ToLower().Equals("and"))
                _mode |= Modes.AND;
            else if (el.GetAttribute("mode").ToLower().Equals("or"))
                _mode |= Modes.OR;

            if (el.GetAttribute("PersonOrganization") != "")
                _mode |= Modes.PersonOrganization;
            if (el.GetAttribute("PersonPhysical") != "")
                _mode |= Modes.PersonPhysical;
            if (el.GetAttribute("PersonBank") != "")
                _mode |= Modes.PersonBank;

            if (el.GetAttribute("PersonCheck") != "")
                _mode |= Modes.PersonCheck;
            if (el.GetAttribute("PersonUncheck") != "")
                _mode |= Modes.PersonUncheck;

            //страна регистрации
            if (el.GetAttribute("PersonArea") != "")
            {
                _mode |= Modes.PersonArea;
                iDArea = Convert.ToInt32(el.GetAttribute("PersonArea"));
                nameArea = Convert.ToString(el.GetAttribute("PersonAreaName"));
            }

            //Ограничения по типам лиц
            if (el.GetAttribute("PersonThemes") != "")
            {
                _mode |= Modes.PersonThemes;
                if (el.GetAttribute("PersonThemes") != "-1")
                {
                    string strTypes = el.GetAttribute("PersonThemes");
                    string[] types = strTypes.Split(';');
                    for (int n = 0; n < types.Length; n += 2)
                    {
                        ListTypes.Add(types[n], types[n + 1]);
                    }
                    if (el.GetAttribute("PersonSubThemes") != "")
                        _mode |= Modes.PersonSubThemes;
                }
            }

            //Ответственные сотрудники
            if (el.GetAttribute("PersonUsers") != "")
            {
                _mode |= Modes.PersonUsers;
                string strUsers = el.GetAttribute("PersonUsers");
                string[] users = strUsers.Split(';');
                for (int n = 0; n < users.Length; n += 2)
                {
                    ListUsers.Add(users[n], users[n + 1]);
                }
            }

            //организационно правовая форма
            if (el.GetAttribute("PersonOPForma") != "")
            {
                _mode |= Modes.PersonOPForma;
                iDOPForm = Convert.ToInt32(el.GetAttribute("PersonOPForma"));
                nameOPForm = Convert.ToString(el.GetAttribute("PersonOPFormaName"));
            }
            //бизнес проекты

            if (el.GetAttribute("PersonAnyBusinessProject") != "")
                _mode |= Modes.PersonAnyBusinessProject;
            else if (el.GetAttribute("PersonBusinessProject") != "")
            {
                _mode |= Modes.PersonBusinessProject;
                iDBusinessProject = Convert.ToInt32(el.GetAttribute("PersonBusinessProject"));
                nameBusinessProject = Convert.ToString(el.GetAttribute("PersonBusinessProjectName"));
                if (el.GetAttribute("PersonSubBusinessProject") != "")
                    _mode |= Modes.PersonSubBusinessProject;
            }
            else if (el.GetAttribute("PersonNullBusinessProject") != "")
            {
                _mode |= Modes.PersonNullBusinessProject;
            }

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
