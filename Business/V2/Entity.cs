using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Kesco.Lib.Win.Data.Business.V2
{
    public delegate string GetStringValue();

    public delegate void SetStringValue(string s);

    public delegate void PropChangedEventHandler(object sender, PropChangedEventArgs e);

    public delegate void LoadDelegate();

    public class Entity
    {
        public class CmdParam
        {
            private string name;

            public string Name
            {
                get { return name; }
            }

            private string value;

            public string Value
            {
                get { return value; }
            }

            public CmdParam(string name, string value)
            {
                this.name = name;
                this.value = value;
            }
        }

        protected DBModule module;
        public const string strPropID = "id";
        public const string strPropLongLiver = "!!long-liver";
        public const string strPropUnavailable = "unavailable";

        public const string strElEntity = "Entity";
        public const string strElEntityAtrID = "id";
        public const string strElProperty = "P";
        public const string strElPropertyAtrKey = "k";
        public const string strElPropertyAtrValue = "v";

        public const string strValTrue = "1";
        public const string strValFalse = "0";

        private static Hashtable globalDict = new Hashtable();
        protected StringDictionary props;
        private string prefix;


        protected int modifiedCounter;
                      //используется при генерации batch'ей на изм. данных, для подсчета сделанных изменений,

        //для определения есть ли в документе изменения в холостую генерится batch на сохранение и анализируется значение этой переменной

        public string _ID
        {
            get { return props[strPropID]; }
        }

        public virtual StringDictionary TypeMapping
        {
            get { return null; }
        }

        public int ID
        {
            get { return int.Parse(_ID); }
        }

        public bool Unavailable
        {
            get
            {
                if (!props.ContainsKey(strPropUnavailable))
                    Load();
                return props[strPropUnavailable].Equals(strValTrue);
            }
        }

        public bool LongLiver
        {
            set
            {
                if (value)
                    props[strPropLongLiver] = strValTrue;
                else
                    props.Remove(strPropLongLiver);
            }
            get { return props[strPropLongLiver] == "1"; }
        }

        public virtual bool New
        {
            get { return !(_ID.Length > 0 && ID > 0); }
        }

        public virtual bool Modified
        {
            get
            {
                modifiedCounter = 0;
                var w = new StringWriter();
                RenderCmdTextSave(w);
                return modifiedCounter > 0;
            }
        }

        public int ModifiedCounter
        {
            get { return modifiedCounter; }
        }

        public bool IsPropModified(string name)
        {
            if (!props.ContainsKey("@" + name))
                return false;
            if (!props.ContainsKey(name))
                return true;
            return !props["@" + name].Equals(props[name]);
        }


        public string GetProp(string name, LoadDelegate load)
        {
            return GetProp(name, name, load);
        }

        public string GetProp(string name, string name2check, LoadDelegate load)
        {
            if (props.ContainsKey("@" + name2check))
                return props["@" + name];
            if (!props.ContainsKey(name2check))
            {
                if (!New)
                    load(); //пытаемся загрузить
                if (!props.ContainsKey(name2check))
                    return props["@" + name] = ""; //автоинициализация (если не загрузили)
            }
            return props[name];
        }

        public string GetProp(string name)
        {
            return GetProp(name, Load);
        }

        public void SetProp(string name, string value)
        {
            string v = GetProp(name);
            if (v.Equals(value))
                return;

            props["@" + name] = value;
            if (props.ContainsKey(name) && props[name] == value)
                props.Remove("@" + name); //если новое значение соответствует оригинальному, затираем его
            OnPropChanged(new PropChangedEventArgs(name, v, value));
        }

        public event PropChangedEventHandler PropChanged;

        protected void OnPropChanged(PropChangedEventArgs e)
        {
            if (PropChanged != null) PropChanged(this, e);
        }

        public StringCollection GetInsertedItems(string propertyName)
        {
            var col = new StringCollection();
            if (!props.ContainsKey("@" + propertyName))
                return col; //никаких изменений не внесено

            StringCollection nw = Str2Collection(props["@" + propertyName]);
            StringCollection ld = Str2Collection(props.ContainsKey(propertyName) ? props[propertyName] : "");
            foreach (string id in nw.Cast<string>().Where(id => !ld.Contains(id)))
                col.Add(id);

            return col;
        }

        public StringCollection GetDeletedItems(string propertyName)
        {
            var col = new StringCollection();
            if (!props.ContainsKey("@" + propertyName))
                return col; //никаких изменений не внесено
            StringCollection nw = Str2Collection(props["@" + propertyName]);
            StringCollection ld = Str2Collection(props.ContainsKey(propertyName) ? props[propertyName] : "");
            foreach (string id in ld.Cast<string>().Where(id => !nw.Contains(id)))
                col.Add(id);
            return col;
        }

        public string Serialize()
        {
            return props.Cast<DictionaryEntry>().Aggregate("",
                                                           (current, de) =>
                                                           current +
                                                           ((string) de.Key + "=" +
                                                            HttpUtility.UrlEncode((string) de.Value, Encoding.Default) +
                                                            "&"));
        }

        public void Deserialize(string s)
        {
            MatchCollection m = Regex.Matches(s, "(?<=\\?|&|^)([@a-z0-9]+)=([^&]{0,})(?=&|$)", RegexOptions.IgnoreCase);
            string key;
            string val;
            for (int i = 0; i < m.Count; i++)
            {
                key = m[i].Groups[1].Value;
                if (key == "id")
                    continue;
                val = HttpUtility.UrlDecode(m[i].Groups[2].Value, Encoding.Default);

                props[key] = val;
            }
        }

        public virtual void Init()
        {

        }

        public virtual void Load()
        {
            props[strPropUnavailable] = strValTrue;
        }

        public void Populate(DataRow row, StringDictionary mapping)
        {
            string key;
            foreach (
                DataColumn col in row.Table.Columns.Cast<DataColumn>().Where(col => mapping.ContainsKey(col.ColumnName))
                )
            {
                key = mapping[col.ColumnName];
                props[key] = row.IsNull(col.ColumnName)
                                 ? string.Empty
                                 : Object2Str(row[col.ColumnName]);
            }
            props[strPropUnavailable] = strValFalse;
        }

        public void Populate(IDataReader reader, StringDictionary mapping)
        {
            string key;
            for (int i = 0; i < reader.FieldCount; i++)
                if (mapping.ContainsKey(reader.GetName(i)))
                {
                    key = mapping[reader.GetName(i)];
                    props[key] = reader.IsDBNull(i) ? string.Empty : Object2Str(reader.GetValue(i));
                }
            props[strPropUnavailable] = strValFalse;
        }

        public static string NameValueCollection2Str(NameValueCollection col)
        {
            return NameValueCollection2Str(col, ";", ";");
        }

        public static string NameValueCollection2Str(NameValueCollection col, string separatorItems,
                                                     string separatorPair)
        {
            var b = new StringBuilder();
            int i = 0;
            foreach (string key in col.Keys)
            {
                if (i > 0)
                    b.Append(separatorItems);
                b.Append(key);
                b.Append(separatorPair);
                b.Append(col[key]);
                i++;
            }
            return b.ToString();
        }

        public static string Collection2Str(StringCollection col)
        {
            var b = new StringBuilder();
            for (int i = 0; i < col.Count; i++)
            {
                if (i > 0) b.Append(",");
                b.Append(col[i]);
            }
            return b.ToString();
        }

        public static string Decimal2Str(decimal val)
        {
            return DecimalStr2Str(val.ToString().Replace(",", "."));
        }

        public static string Double2Str(double val)
        {
            return DecimalStr2Str(val.ToString().Replace(",", "."));
        }

        private static string DecimalStr2Str(string s)
        {
            Match m = Regex.Match(s, "[.]\\d*(0+)$", RegexOptions.RightToLeft);
            if (m.Success)
                s = s.Remove(m.Groups[1].Index, m.Groups[1].Length);
            m = Regex.Match(s, "[.]$");
            if (m.Success)
                s = s.Remove(m.Index, 1);
            m = Regex.Match(s, "^-?(0+)\\d+");
            if (m.Success)
                s = s.Remove(m.Groups[1].Index, m.Groups[1].Length);
            return s;
        }

        public static string DateTime2Str(DateTime val)
        {
            return Regex.Replace((val).ToString("yyyyMMddHHmmss"), "000000$", "");
        }

        public static string Time2Str(DateTime val)
        {
            return (val).ToString("HH:mm:ss");
        }

        public static string Bool2Str(bool val)
        {
            return val ? "1" : "0";
        }

        public static string Object2Str(object val)
        {
            if (val is bool)
                return Bool2Str((bool) val);
            if (val is decimal)
                return Decimal2Str((decimal) val);
            if (val is double || val is float)
                return Double2Str((double) val);
            if (val is DateTime)
                return DateTime2Str((DateTime) val);
            if (val is StringCollection)
                return Collection2Str((StringCollection) val);

            return val.ToString();
        }

        public static decimal Str2Decimal(string val, decimal defaultValue)
        {
            return string.IsNullOrEmpty(val) ? defaultValue : Str2Decimal(val);
        }

        public static decimal Str2Decimal(string val)
        {
            try
            {
                var nfi = (NumberFormatInfo) NumberFormatInfo.CurrentInfo.Clone();
                nfi.NumberDecimalSeparator = ".";
                return Convert.ToDecimal(val.Replace(',', '.'), nfi);

            }
            catch
            {
                throw new Exception("Невозможно преобразовать строку '" + val + "' к типу Decimal");
            }
        }

        public static double Str2Double(string val, double defaultValue)
        {
            return string.IsNullOrEmpty(val) ? defaultValue : Str2Double(val);
        }

        public static double Str2Double(string val)
        {
            return double.Parse(val.Replace(".", ","));
        }

        public static DateTime Str2DateTime(string val, DateTime defaultValue)
        {
            return string.IsNullOrEmpty(val) ? defaultValue : Str2DateTime(val);
        }

        public static DateTime Str2DateTime(string val)
        {
            return DateTime.ParseExact(val.PadRight(14, '0'), "yyyyMMddHHmmss",
                                       CultureInfo.InvariantCulture);
        }

        public static DateTime Str2Time(string val)
        {
            return DateTime.ParseExact(val.PadRight(6, '0'), "HHmmss",
                                       CultureInfo.InvariantCulture);
        }

        public static bool Str2Bool(string val, bool defaultValue)
        {
            return string.IsNullOrEmpty(val) ? defaultValue : Str2Bool(val);
        }

        public static bool Str2Bool(string val)
        {
            return val.Equals("1");
        }

        public static Regex regex4KeysCollection = new Regex("[A-ZА-Я0-9_]+", RegexOptions.IgnoreCase);
        public static Regex regex4IntCollection = new Regex("-?\\d+", RegexOptions.IgnoreCase);
        public static Regex regex4DefaultCollection = new Regex("-?\\d+|@[a-z0-9]+|[_а-я0-9]+", RegexOptions.IgnoreCase);

        public static StringCollection Str2Collection(string val)
        {
            return Str2Collection(val, regex4DefaultCollection);
        }

        public static StringCollection Str2Collection(string val, Regex regex)
        {
            var col = new StringCollection();
            MatchCollection m = regex.Matches(val);

            for (int i = 0; i < m.Count; i++) col.Add(m[i].Value);
            return col;
        }


        #region Округление

        public static float Round(float numToRound, int numOfDec)
        {
            return (float) Round((decimal) numToRound, numOfDec);
        }

        public static double Round(double numToRound, int numOfDec)
        {
            return (double) Round((decimal) numToRound, numOfDec);
        }

        public static decimal Round(decimal numToRound, int numOfDec)
        {
            if (numOfDec < 0)
                throw new ArgumentException("Число десятичных знаков должно быть больше или равно 0");

            return decimal.Parse(numToRound.ToString("N" + numOfDec));
        }

        #endregion

        public virtual void Save()
        {
            string newID = module.ExecuteScalar(SaveCommandText);

            if (New)
                ChangeID(newID);
            ClearProps();
        }

        public virtual void Delete()
        {
            string newID = module.ExecuteScalar(DeleteCommandText);

            Hashtable contextDict = GetContextDict();
            contextDict.Remove(prefix + _ID);
            props = null;
        }

        public void SaveAs(string newID)
        {
            ChangeID(newID);
            Save();
        }

        public string SaveCommandText
        {
            get
            {
                var w = new StringWriter();
                RenderCmdTextSave(w);
                return w.ToString();
            }
        }

        public string DeleteCommandText
        {
            get
            {
                var w = new StringWriter();
                RenderCmdTextDelete(w);
                return w.ToString();
            }
        }

        protected virtual void RenderCmdTextSave(StringWriter w)
        {
            w.Write("\nDECLARE  @id int");

            if (Unavailable)
            {
                RenderCmdTextInsert(w);
            }
            else
            {
                w.Write("\nSET  @id={0}", _ID);
                RenderCmdTextUpdate(w);
            }
            w.Write("\n\nSELECT @id");
        }

        protected virtual void RenderCmdTextInsert(StringWriter w)
        {

        }

        protected virtual void RenderCmdTextUpdate(StringWriter w)
        {

        }

        public void RenderCmdTextUpdate(StringWriter w, string table, ArrayList args, CmdParam key)
        {
            w.Write("\nUPDATE {0} SET", table);
            var last = (CmdParam) args[args.Count - 1];
            foreach (CmdParam arg in args)
            {
                w.Write("\n{0}={1}", arg.Name, arg.Value);
                if (arg != last) w.Write(",");
            }
            w.Write("\nWHERE {0}={1}", key.Name, key.Value);
        }

        public void RenderCmdTextInsert(StringWriter w, string table, ArrayList args)
        {
            var last = (CmdParam) args[args.Count - 1];

            w.Write("\nINSERT {0}(", table);
            foreach (CmdParam arg in args)
            {
                w.Write(arg.Name);
                if (arg != last) w.Write(",");
            }
            w.Write(")\nVALUES(");
            foreach (CmdParam arg in args)
            {
                w.Write(arg.Value);
                if (arg != last) w.Write(",");
            }
            w.Write(")");
        }

        public virtual void RenderCmdTextDelete(StringWriter w)
        {

        }

        protected void RenderCmdTextInsert(StringWriter w, string tableName)
        {
            int n = 0;
            int colpos = -1;
            int pos = w.GetStringBuilder().Length;

            w.Write("\nINSERT {0}(", tableName);
            colpos = w.GetStringBuilder().Length;
            w.Write("\n)VALUES(");
            RenderCmdTextInsertUpdateParameters(w, ref colpos, ref n);
            w.Write(")");
            w.Write("\nSET  @id=SCOPE_IDENTITY()");

            if (n > 0) modifiedCounter += n;
            else w.GetStringBuilder().Length = pos;
        }

        protected void RenderCmdTextUpdate(StringWriter w, string tableName, string pkName)
        {
            int n = 0;
            int colpos = -1;
            int pos = w.GetStringBuilder().Length;

            w.Write("\nUPDATE {0} SET", tableName);
            RenderCmdTextInsertUpdateParameters(w, ref colpos, ref n);
            w.Write("\nWHERE {0}={1}", pkName, _ID);

            if (n == 0) w.GetStringBuilder().Length = pos; //отменяем чего записали
            else modifiedCounter += n;
        }

        private static NumberFormatInfo sqlLiteralDecimalFormat;

        public static string GetSqlLiteral(string s)
        {
            return "'" + s.Replace("'", "''") + "'";
        }

        public static string GetSqlLiteral(DateTime dt)
        {
            return dt == DateTime.MaxValue ? "NULL" : "'" + dt.ToString("yyyyMMdd HH:mm:ss") + "'";
        }

        public static string GetSqlLiteral(long l)
        {
            return l == long.MaxValue ? "NULL" : l.ToString();
        }

        public static string GetSqlLiteral(decimal d)
        {
            return d == decimal.MaxValue ? "NULL" : d.ToString("N", sqlLiteralDecimalFormat);
        }

        static Entity()
        {
            sqlLiteralDecimalFormat = (NumberFormatInfo) NumberFormatInfo.InvariantInfo.Clone();
            sqlLiteralDecimalFormat.NumberDecimalSeparator = ".";
            sqlLiteralDecimalFormat.NumberGroupSeparator = "";
        }

        protected virtual void RenderCmdTextInsertUpdateParameters(StringWriter w, ref int colpos, ref int n)
        {

        }

        protected void RenderCmdTextParameter(StringWriter w, ref int colpos, ref int n, string column, string value)
        {
            RenderCmdTextParameter(w, ref colpos, ref n, column, value, "");
        }

        protected void RenderCmdTextParameter(StringWriter w, ref int colpos, ref int n, string column, string value,
                                              string type)
        {
            StringBuilder b = w.GetStringBuilder();

            if (colpos > 0)
            {
                column = (n == 0 ? "" : ",") + "\n" + column;
                b.Insert(colpos, column);
                colpos += column.Length;

                if (n > 0) w.Write(",");
                w.Write("\n");
            }
            else
            {
                if (n > 0)
                    if (n > 0) w.Write(",");
                w.Write("\n");
                w.Write(column);
                w.Write("=");
            }


            switch (type)
            {
                case "dt":
                    RenderSqlValueDate(w, value);
                    break;
                case "st":
                    RenderSqlValueText(w, value);
                    break;
                default:
                    RenderSqlValue(w, value);
                    break;
            }

            n++;
        }

        protected void RenderSqlAssignment(StringWriter w, DictionaryEntry de)
        {
            RenderSqlColumn(w, de);
            w.Write("=");
            RenderSqlValue(w, de);
        }

        protected void RenderSqlColumn(StringWriter w, DictionaryEntry de)
        {
            var prop = (string) de.Key;
            var column = (string) de.Value;

            w.Write(column);
        }

        protected void RenderSqlValue(StringWriter w, DictionaryEntry de)
        {
            var prop = (string) de.Key;
            var column = (string) de.Value;

            Type t = GetType();
            PropertyInfo pi = t.GetProperty("_" + prop,
                                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (pi == null)
                throw new Exception(t + " doesn't contains property " + "_" + prop);

            var x = (string) pi.GetValue(this, null);
            x = x ?? "";

            switch (TypeMapping[prop])
            {
                case "dt":
                    RenderSqlValueDate(w, x);
                    break;
                case "st":
                    RenderSqlValueText(w, x);
                    break;
                default:
                    RenderSqlValue(w, x);
                    break;
            }
        }

        protected void RenderSqlValue(StringWriter w, string v)
        {
            w.Write(!string.IsNullOrEmpty(v) ? v : "NULL");
        }

        protected void RenderSqlValueText(StringWriter w, string v)
        {
            if (v == null)
                w.Write("''");
            else
                w.Write("'" + v.Replace("'", "''") + "'");
        }

        protected void RenderSqlValueDate(StringWriter w, string v)
        {
            if (string.IsNullOrEmpty(v))
                w.Write("NULL");
            else
                w.Write("'" + Str2DateTime(v).ToString("yyyyMMdd HH:mm:ss") + "'");
        }

        public virtual void RenderCmdTextOnError(StringWriter w)
        {
            w.Write(
                "\nIF @@ERROR > 0 BEGIN RAISERROR('При сохранении изменений в документе произошли ошибки, требуется проверка документа.',16,1) RETURN END");
        }

        public static string GetNewID()
        {
            int newID = 0;
            int x;
            Match m;
            Regex re = new Regex("-\\d+$");

            Hashtable ht = GetContextDict();

            foreach (DictionaryEntry de in ht)
                if ((m = re.Match((string)de.Key)).Success)
                    if (newID > (x = int.Parse(m.Value)))
                        newID = x;

            return (--newID).ToString();
        }

        public static Hashtable GetContextDict()
        {
            Hashtable contextDict;
            HttpContext context = HttpContext.Current;

            if (context == null) contextDict = globalDict;
            else if ((contextDict = (Hashtable) context.Items["contextDict"]) == null)
                context.Items["contextDict"] = contextDict = new Hashtable();
            return contextDict;
        }

        public static void SaveToXmlElement(XmlElement el)
        {
            Hashtable contextDict = GetContextDict();

            StringDictionary props;
            XmlDocument d = el.OwnerDocument;
            XmlElement entity;
            XmlElement p;

            XmlNodeList list = el.SelectNodes(strElEntity);
            for (int i = list.Count - 1; i >= 0; i--) el.RemoveChild(list[i]);
            foreach (DictionaryEntry de in contextDict)
            {
                props = (StringDictionary) de.Value;
                if (!props.ContainsKey(strPropLongLiver)) continue;

                el.AppendChild(entity = d.CreateElement(strElEntity));
                entity.SetAttribute(strElEntityAtrID, (string) de.Key);
                foreach (DictionaryEntry de2 in props)
                {
                    if (((string) de2.Key)[0] == '#') continue;
                    entity.AppendChild(p = entity.OwnerDocument.CreateElement(strElProperty));
                    p.SetAttribute(strElPropertyAtrKey, (string) de2.Key);
                    p.SetAttribute(strElPropertyAtrValue, (string) de2.Value);
                }
            }
        }

        public static void LoadFromXmlElement(XmlElement el)
        {
            Hashtable contextDict = GetContextDict();

            StringDictionary props;
            if (el.SelectNodes(strElEntity) != null)
                foreach (XmlElement entity in el.SelectNodes(strElEntity))
                {
                    if (contextDict.ContainsKey(entity.GetAttribute(strElEntityAtrID)))
                        props = (StringDictionary) contextDict[entity.GetAttribute(strElEntityAtrID)];
                    else
                        contextDict[entity.GetAttribute(strElEntityAtrID)] = (props = new StringDictionary());

                    if (entity.SelectNodes(strElEntity) != null)
                        foreach (XmlElement p in entity.SelectNodes(strElProperty))
                            props[p.GetAttribute(strElPropertyAtrKey)] = p.GetAttribute(strElPropertyAtrValue);
                }
        }

        public Entity()
        {

        }

        public Entity(string id, string prefix, DBModule module)
        {
            this.module = module;
            this.prefix = prefix;

            string key = prefix + id;
            if (globalDict.ContainsKey(key))
                props = (StringDictionary) globalDict[key];
            else
            {
                Hashtable contextDict = GetContextDict();

                if (contextDict.ContainsKey(key))
                    props = (StringDictionary) contextDict[key];
                else
                {
                    contextDict[key] = props = new StringDictionary();
                    props.Add(strPropID, id);
                    if (New)
                        Init();
                }
            }
        }


        protected void ChangeID(string newID)
        {
            Hashtable contextDict = GetContextDict();

            contextDict.Remove(prefix + _ID);
            contextDict.Remove(prefix + newID);
            contextDict[prefix + newID] = props;
            props[strPropID] = newID;
        }

        public void ClearProps()
        {
            string id = props[strPropID];
            bool ll = LongLiver;

            props.Clear();

            props[strPropID] = id;
            if (ll) LongLiver = true;
        }
    }
}
