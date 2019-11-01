//Бизнес-объект: Документ

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;

namespace Kesco.Lib.Win.Data.Business.V2.Docs.DomainObjects
{
    public enum NumGenTypes
    {
        CanNotBeGenerated = 0,
        CanBeGenerated = 1,
        MustBeGenerated = 2,
    }

    /// <summary>
    /// Бизнес-объект - документ
    /// </summary>
    public class Doc : Entity
    {
        public class Link : Entity
        {
            private static StringDictionary defaultMapping;

            public static StringDictionary DefaultMapping
            {
                get { return defaultMapping; }
            }

            public string _Base
            {
                get { return GetProp("base"); }
                set { SetProp("base", value); }
            }

            public string _Sequel
            {
                get { return GetProp("sequel"); }
                set { SetProp("sequel", value); }
            }

            public string _Field
            {
                get { return GetProp("field"); }
                set { SetProp("field", value); }
            }

            public string _BaseOrder
            {
                get { return GetProp("baseOrder"); }
                set { SetProp("baseOrder", value); }
            }

            public string _SequelOrder
            {
                get { return GetProp("sequelOrder"); }
                set { SetProp("sequelOrder", value); }
            }

            public string _Editor
            {
                get { return GetProp("editor"); }
                set { SetProp("editor", value); }
            }

            public string _Edited
            {
                get { return GetProp("edited"); }
                set { SetProp("edited", value); }
            }


            public Doc Base
            {
                get { return !string.IsNullOrEmpty(_Base) ? new Doc(_Base) : null; }
            }

            public Field Field
            {
                get { return !string.IsNullOrEmpty(_Field) ? new Field(_Field) : null; }
            }


            static Link()
            {
                defaultMapping = new StringDictionary
                                     {
                                         {"КодСвязиДокументов", "id"},
                                         {"КодДокументаОснования", "base"},
                                         {"КодДокументаВытекающего", "sequel"},
                                         {"КодПоляДокумента", "field"},
                                         {"ПорядокОснования", "baseOrder"},
                                         {"ПорядокВытекающего", "sequelOrder"},
                                         {"Изменил", "editor"},
                                         {"Изменено", "edited"}
                                     };
            }

            public Link(string id) : base(id, "DocLink", Env.Docs)
            {

            }
        }

        private static StringDictionary defaultMapping;
        private static StringDictionary dataMapping;

        private static StringDictionary typeMappting;

        private static StringDictionary propColMapping;

        protected ArrayList links2insert = new ArrayList();

        public static StringDictionary DefaultMapping
        {
            get { return defaultMapping; }
        }

        public static StringDictionary DataMapping
        {
            get { return dataMapping; }
        }

        public override StringDictionary TypeMapping
        {
            get { return typeMappting; }
        }

        public string GetPropData(string name)
        {
            return GetProp(name, LoadData);
        }

        public string GetPropData(Field field)
        {
            return field == null ? "" : GetPropData(dataMapping[field._ColumnName]);
        }

        public void SetProp(Field field, string value)
        {
            SetProp(dataMapping[field._ColumnName], value);
        }

        #region _Properties

        public string _Type
        {
            get { return GetProp("type"); }
            set { SetProp("type", value); }
        }

        public string _Number
        {
            get { return GetProp("number"); }
            set { SetProp("number", value); }
        }

        public string _Date
        {
            get { return GetProp("date"); }
            set { SetProp("date", value); }
        }

        public string _Description
        {
            get { return GetProp("description"); }
            set { SetProp("description", value); }
        }

        public string _MainImage
        {
            get { return GetProp("mainImage"); }
            set { SetProp("mainImage", value); }
        }

        public string _Editor
        {
            get { return GetProp("editor"); }
            set { SetProp("editor", value); }
        }

        public string _Edited
        {
            get { return GetProp("edited"); }
            set { SetProp("edited", value); }
        }


        public string _DataEditor
        {
            get { return GetPropData("dataEditor"); }
            set { SetProp("dataEditor", value); }
        }

        public string _DataEdited
        {
            get { return GetPropData("dataEdited"); }
            set { SetProp("dataEdited", value); }
        }

        public string _Person1
        {
            get { return GetPropData("person1"); }
            set { SetProp("person1", value); }
        }

        public string _Person2
        {
            get { return GetPropData("person2"); }
            set { SetProp("person2", value); }
        }

        public string _Person3
        {
            get { return GetPropData("person3"); }
            set { SetProp("person3", value); }
        }

        public string _Person4
        {
            get { return GetPropData("person4"); }
            set { SetProp("person4", value); }
        }

        public string _Sklad1
        {
            get { return GetPropData("sklad1"); }
            set { SetProp("sklad1", value); }
        }

        public string _Sklad2
        {
            get { return GetPropData("sklad2"); }
            set { SetProp("sklad2", value); }
        }

        public string _Sklad3
        {
            get { return GetPropData("sklad3"); }
            set { SetProp("sklad3", value); }
        }

        public string _Sklad4
        {
            get { return GetPropData("sklad4"); }
            set { SetProp("sklad4", value); }
        }

        public string _Resource1
        {
            get { return GetPropData("resource1"); }
            set { SetProp("resource1", value); }
        }

        public string _Resource2
        {
            get { return GetPropData("resource2"); }
            set { SetProp("resource2", value); }
        }

        public string _Employee1
        {
            get { return GetPropData("employee1"); }
            set { SetProp("employee1", value); }
        }

        public string _Employee2
        {
            get { return GetPropData("employee2"); }
            set { SetProp("employee2", value); }
        }

        public string _Employee3
        {
            get { return GetPropData("employee3"); }
            set { SetProp("employee3", value); }
        }

        public string _BProject
        {
            get { return GetPropData("bproject"); }
            set { SetProp("bproject", value); }
        }

        public string _BazisPostavki
        {
            get { return GetPropData("bazispostavki"); }
            set { SetProp("bazispostavki", value); }
        }

        public string _VidTransporta
        {
            get { return GetPropData("vidtransporta"); }
            set { SetProp("vidtransporta", value); }
        }

        public string _StoragePlace
        {
            get { return GetPropData("pl"); }
            set { SetProp("pl", value); }
        }

        public string _Unit
        {
            get { return GetPropData("unit"); }
            set { SetProp("unit", value); }
        }

        public string _RateVAT
        {
            get { return GetPropData("vat"); }
            set { SetProp("vat", value); }
        }

        public string _TRWesel1
        {
            get { return GetPropData("tu1"); }
            set { SetProp("tu1", value); }
        }

        public string _TRWesel2
        {
            get { return GetPropData("tu2"); }
            set { SetProp("tu2", value); }
        }

        public string _PosZvkBuy
        {
            get { return GetPropData("poszvkbuy"); }
            set { SetProp("poszvkbuy", value); }
        }

        public string _Area
        {
            get { return GetPropData("area"); }
            set { SetProp("area", value); }
        }

        public string _TextDoc
        {
            get { return GetPropData("textdoc"); }
            set { SetProp("textdoc", value); }
        }

        public string _Persons
        {
            get { return GetProp("persons", LoadPersons); }
        }

        public string _GetBaseDocs(string field)
        {
            if (props.ContainsKey("@baseDocs" + field)) return props["@baseDocs" + field];

            if (!props.ContainsKey("baseDocsAll") && !New) LoadBaseDocs();

            if (!props.ContainsKey("baseDocs" + field)) return props["@baseDocs" + field] = ""; //автоинициализация
            return props["baseDocs" + field];
        }

        public string _GetBaseDoc(string field)
        {
            StringCollection col = Str2Collection(_GetBaseDocs(field));
            return col.Count > 0 ? col[0] : "";
        }

        public string _GetSequelDocs(string field)
        {
            if (props.ContainsKey("@sequelDocs" + field)) return props["@sequelDocs" + field];

            if (!props.ContainsKey("sequelDocsAll") && !New) LoadSequelDocs();

            if (!props.ContainsKey("sequelDocs" + field)) return props["@sequelDocs" + field] = ""; //автоинициализация
            return props["sequelDocs" + field];
        }

        public void _SetSequelDocs(string field, string value)
        {
            SetProp("sequelDocs" + field, value);
        }

        public string _Subscribe
        {
            get { return props["subscribe"]; }
            set { props["subscribe"] = value; }
        }

        public string _DocViewIP
        {
            get { return props["docviewip"]; }
            set { props["docviewip"] = value; }
        }

        public string _DocViewPort
        {
            get { return props["docviewport"]; }
            set { props["docviewport"] = value; }
        }

        #endregion

        public DateTime Date
        {
            get { return Str2DateTime(_Date, DateTime.MinValue); }
        }

        public DateTime DataEdited
        {
            get { return Str2DateTime(_DataEdited, DateTime.MinValue); }
        }

        public DocType Type
        {
            get { return _Type.Length == 0 ? null : new DocType(_Type); }
        }

        public string FullName
        {
            get
            {
                var b = new StringBuilder();

                if (_Type.Length > 0)
                    b.Append(Type.Name);

                if (New)
                    b.Append(" (новый документ)");
                else
                {

                    if (Unavailable)
                        b.Append("#" + _ID);
                    else
                    {
                        if (_Number.Length > 0) b.Append(" № " + _Number);
                        if (_Date.Length > 0) b.Append(" от " + Date.ToString("dd.MM.yyyy"));
                    }
                }
                return b.ToString();
            }
        }

        public string FullNameEng
        {
            get
            {
                var b = new StringBuilder();

                if (_Type.Length > 0)
                {
                    DocType t = Type;
                    if (t.Unavailable)
                        b.AppendFormat("DocType #{0}", _Type);
                    else if (t._NameEng.Length == 0)
                        b.Append(Type.NameEng);
                    else
                        b.Append(Type.Name);
                }

                if (New)
                    b.Append(" (new document)");
                else
                {
                    if (_Number.Length > 0)
                        b.Append(" № " + _Number);
                    if (_Date.Length > 0)
                        b.Append(" dd " + Date.ToString("dd.MM.yyyy"));
                }

                return b.ToString();
            }
        }

        public string Description
        {
            get { return _Description; }
        }

        public Field GetFieldByName(string name)
        {
            name = name.ToLower().Replace(" ", "");
            StringCollection col = Str2Collection(Type._Fields);

            return
                (from string t in col select new Field(t)).FirstOrDefault(
                    f => f._Name.ToLower().Replace(" ", "").Equals(name));
        }

        public bool DataUnavailable
        {
            get
            {
                if (!props.ContainsKey("dataUnavailable")) LoadData();
                return props["dataUnavailable"].Equals("1");
            }
        }

        public bool HasChanges(string[] keys)
        {
            return keys.Any(IsPropModified);
        }

        public override void Init()
        {
            _Number = "";
            _Date = "";
            _Description = "";

            foreach (DictionaryEntry de in dataMapping)
                SetProp((string) de.Value, "");

        }

        public override void Load()
        {
            var dt = new DataTable();
            Env.Docs.Find("SELECT * FROM vwДокументы WHERE КодДокумента=" + _ID, dt);
            if (dt.Rows.Count == 1) Populate(dt.Rows[0], defaultMapping);
            else props["unavailable"] = "1";

        }

        public void LoadData()
        {
            var dt = new DataTable();
            Env.Docs.Find("SELECT * FROM vwДокументыДанные WHERE КодДокумента=" + _ID, dt);
            if (dt.Rows.Count == 1)
            {
                Populate(dt.Rows[0], dataMapping);
                props["dataUnavailable"] = "0";
            }
            else props["dataUnavailable"] = "1";

        }

        public void LoadBaseDocs()
        {
            var colLinks = new StringCollection();
            var col = new StringCollection();
            StringCollection col1;
            var ht = new Hashtable();
            if (!New)
            {
                var dt = new DataTable();
                Env.Docs.Find(
                    "SELECT * FROM vwСвязиДокументов WHERE КодДокументаВытекающего=" + _ID +
                    " ORDER BY ПорядокОснования", dt);
                foreach (DataRow row in dt.Rows)
                {
                    Link link = new Link(row["КодСвязиДокументов"].ToString());
                    link.Populate(row, Link.DefaultMapping);

                    col.Add(link._Base);
                    colLinks.Add(link._ID);

                    string key = "baseDocs" + link._Field;
                    if (!ht.ContainsKey(key)) ht[key] = col1 = new StringCollection();
                    else col1 = (StringCollection) ht[key];
                    col1.Add(link._Base);
                }
            }

            props["baseDocsLinks"] = Collection2Str(colLinks);
            props["baseDocsAll"] = Collection2Str(col);
            foreach (DictionaryEntry de in ht)
                props[(string) de.Key] = Collection2Str((StringCollection) de.Value);
        }

        public void LoadSequelDocs()
        {
            var col = new StringCollection();
            StringCollection col1;
            var ht = new Hashtable();

            if (!New)
            {
                var dt = new DataTable();
                Env.Docs.Find(
                    "SELECT * FROM vwСвязиДокументов WHERE КодДокументаОснования=" + _ID +
                    " ORDER BY ПорядокВытекающего", dt);

                foreach (DataRow row in dt.Rows)
                {
                    var link = new Link(row["КодСвязиДокументов"].ToString());
                    link.Populate(row, Link.DefaultMapping);

                    col.Add(link._Sequel);

                    var key = "sequelDocs" + link._Field;
                    if (!ht.ContainsKey(key)) ht[key] = col1 = new StringCollection();
                    else col1 = (StringCollection) ht[key];
                    col1.Add(link._Sequel);
                }
            }

            props["sequelDocsAll"] = Collection2Str(col);
            foreach (DictionaryEntry de in ht)
                props[(string) de.Key] = Collection2Str((StringCollection) de.Value);
        }

        public void LoadPersons()
        {
            var col = new StringCollection();
            if (!New)
            {
                var dt = new DataTable();
                Env.Docs.Find("SELECT * FROM vwЛицаДокументов WHERE КодДокумента=" + _ID, dt);
                foreach (DataRow row in dt.Rows)
                    col.Add(row["КодЛица"].ToString());
            }

            props["persons"] = Collection2Str(col);
        }

        static Doc()
        {
            typeMappting = new StringDictionary
                               {
                                   {"number", "st"},
                                   {"date", "dt"},
                                   {"description", "st"},
                                   {"edited", "dt"},
                                   {"dataEdited", "dt"},
                                   {"textdoc", "st"}
                               };

            defaultMapping = new StringDictionary
                                 {
                                     {"КодДокумента", "id"},
                                     {"КодТипаДокумента", "type"},
                                     {"НомерДокумента", "number"},
                                     {"ДатаДокумента", "date"},
                                     {"Описание", "description"},
                                     {"КодИзображенияДокументаОсновного", "mainImage"},
                                     {"Изменил", "editor"},
                                     {"Изменено", "edited"}
                                 };

            dataMapping = new StringDictionary
                              {
                                  {"Изменил", "dataEditor"},
                                  {"Изменено", "dataEdited"},
                                  {"КодЛица1", "person1"},
                                  {"КодЛица2", "person2"},
                                  {"КодЛица3", "person3"},
                                  {"КодЛица4", "person4"},
                                  {"КодСклада1", "sklad1"},
                                  {"КодСклада2", "sklad2"},
                                  {"КодСклада3", "sklad3"},
                                  {"КодСклада4", "sklad4"},
                                  {"КодРесурса1", "resource1"},
                                  {"КодРесурса2", "resource2"},
                                  {"КодСотрудника1", "employee1"},
                                  {"КодСотрудника2", "employee2"},
                                  {"КодСотрудника3", "employee3"},
                                  {"КодРасположения1", "disposition1"},
                                  {"КодБизнесПроекта", "bproject"},
                                  {"КодБазисаПоставки", "bazispostavki"},
                                  {"КодВидаТранспорта", "vidtransporta"},
                                  {"КодМестаХранения", "pl"},
                                  {"КодЕдиницыИзмерения", "unit"},
                                  {"КодСтавкиНДС", "vat"},
                                  {"КодТУзла1", "tu1"},
                                  {"КодТУзла2", "tu2"},
                                  {"КодПозицииЗаявкиНаПокупку", "poszvkbuy"},
                                  {"КодТерритории", "area"},
                                  {"КодСтатьиБюджета", "budgetLine"},
                                  {"КодОборудования", "equipment"},
                                  {"ТекстДокумента", "textdoc"}
                              };



            //----------------------------------------------------------- 

            propColMapping = new StringDictionary
                                 {
                                     {"person1", "_Person1"},
                                     {"person2", "_Person2"},
                                     {"person3", "_Person3"},
                                     {"person4", "_Person4"},
                                     {"sklad1", "_Sklad1"},
                                     {"sklad2", "_Sklad2"},
                                     {"sklad3", "_Sklad3"},
                                     {"sklad4", "_Sklad4"},
                                     {"resource1", "_КодРесурса1"},
                                     {"resource2", "_КодРесурса2"},
                                     {"employee1", "_КодСотрудника1"},
                                     {"employee2", "_КодСотрудника2"},
                                     {"employee3", "_КодСотрудника3"},
                                     {"disposition1", "_КодРасположения1"},
                                     {"bproject", "_BProject"},
                                     {"bazispostavki", "_BazisPostavki"},
                                     {"vidtransporta", "_VidTransporta"},
                                     {"pl", "_StoragePlace"},
                                     {"unit", "_Unit"},
                                     {"vat", "_RateVAT"},
                                     {"tu1", "_TRWesel1"},
                                     {"tu2", "_TRWesel2"},
                                     {"poszvkbuy", "_PosZvkBuy"},
                                     {"area", "_Area"},
                                     {"textdoc", "_TextDoc"}
                                 };

            //-----------------------------------------------------------

        }

        public static string GetPropertyName(string ColumnName)
        {
            return propColMapping[dataMapping[ColumnName]];
        }

        public Doc(string id) : base(id, "Doc", Env.Docs)
        {

        }
    }
}

