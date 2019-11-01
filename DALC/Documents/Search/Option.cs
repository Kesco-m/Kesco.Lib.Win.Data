using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search
{
    public class Option
    {
        protected string emptyValueText;

        public String[] NegativeOption { get; internal set; }
        public String[] NegativeValueOption { get; internal set; }

        private static Dictionary<string, Type> map;
		private static Dictionary<string, Type> subsmap;

        protected XmlElement el;
        protected int groupID;
        protected int groupIndex;
        [Localizable(true)] private OptionAttribute meta;

        protected string shortTextPrefix;
        protected string shortTextPostfix;
        protected string htmlPrefix;
        protected string htmlPostfix;

        protected string htmlLinkPrefix;
        protected string htmlLinkPostfix;

        protected string htmlItemPrefix;
        protected string htmlItemPostfix;
        protected string textItemPrefix;
        protected string textItemPostfix;

        protected string errorText;

        private ResourceManager resources;

        internal ResourceManager Resources
        {
            get { return resources ?? (resources = new ResourceManager(GetType())); }
        }

        protected Option(XmlElement el)
        {
            var resourceManager = new ResourceManager(typeof (Option));
            this.el = el;
            meta = GetMeta(GetType());

            groupID = 0;
            emptyValueText = resourceManager.GetString("emptyValueText");

            errorText = "";


            htmlLinkPrefix = "[<A href=#" + Meta.Name + ">";
            htmlLinkPostfix = "</A>]";

            htmlItemPrefix = "<i>";
            htmlItemPostfix = "</i>";

            textItemPrefix = "";
            textItemPostfix = "";

            shortTextPrefix =
                htmlPrefix = Meta.Description;
            shortTextPostfix =
                htmlPostfix = "";
        }

        #region ACCESSORS

        [Localizable(true)]
        public OptionAttribute Meta
        {
            get { return meta; }
        }

        public int GroupID
        {
            get { return groupID; }
        }

        #endregion

        #region STATIC

        static Option()
        {
			map = new Dictionary<string, Type>();
			subsmap = new Dictionary<string, Type>();
            foreach (Type type in typeof (Option).Assembly.GetTypes())
            {
                if (!type.IsSubclassOf(typeof (Option))) continue;
                OptionAttribute meta = GetMeta(type);
                if (meta == null) continue;
                map.Add(meta.Name, type);
				if(meta.MainOption != null)
					subsmap.Add(meta.Name, meta.MainOption);
            }
        }

        private static Type GetOptionType(string name)
        {
            return map.ContainsKey(name) ? (Type) map[name] : typeof (Option);
        }

        public static OptionAttribute GetMeta(Type type)
        {
            OptionAttribute[] attr = (OptionAttribute[]) type.GetCustomAttributes(typeof (OptionAttribute), false);
            return attr.Length == 1 ? attr[0] : null;
        }

		public List<string> GetSubOptions()
		{
			var types = subsmap.Where(x => x.Value == this.GetType()).Select(x=> x.Key).ToList();
			return types.Count > 0 ? types : null;
		}

        public static Option CreateOption(XmlElement el)
        {
            Type type = GetOptionType(el.GetAttribute("name"));
            var args = new object[] {el};
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

            try
            {
                return (Option) Activator.CreateInstance(type, flags, null, args, null);
            }
            catch
            {
                return (Option) Activator.CreateInstance(typeof (UnsupportedOption), flags, null, args, null);
            }
        }

        #endregion

        public virtual string GetHTML()
        {
            return htmlPrefix + htmlPostfix;
        }

        public virtual string GetShortText()
        {
            return shortTextPrefix + shortTextPostfix;
        }

        public virtual string GetText()
        {
            return htmlPrefix + htmlPostfix;
        }

        public virtual string GetItemText(string key)
        {
            return key;
        }

        public virtual string GetSQL(bool throwOnError)
        {
            return "";
        }

        protected virtual string GetSQLCondition(string pattern)
        {
            return pattern;
        }

        protected virtual string GetSQLParameter(string key)
        {
            return key;
        }

		/// <summary>
		/// ќпредел€ет, используктс€ ли пустое значение.
		/// </summary>
		public virtual bool HasEmpty()
		{
			return false;
		}

        public virtual bool IsValid()
        {
            return true;
        }

        public virtual bool OpenWindow()
        {
            return true;
        }

        public virtual string GetErrorString()
        {
            return errorText;
        }

        public static string GetMap()
        {
            String s = "";

            foreach (string key in map.Keys)
            {
                OptionAttribute meta = GetMeta(GetOptionType(key));
                s += key + ";" + map[key] + ";" + meta.Description + "\n";
            }

            return s;
        }

		internal bool IsSubOption()
		{
			return subsmap.ContainsKey(this.Meta.Name);
		}
	}

    [Localizable(true)]
    public class OptionAttribute : Attribute
    {
		public OptionAttribute(string name, string description)
		{
			Name = name;
			Description = description;
		}

		public OptionAttribute(string name, Type type)
        {
            Name = name;
            var res = new ResourceManager(type);
            try
            {
                string des = res.GetString("Meta.Description");
                if (!string.IsNullOrEmpty(des))
                    Description = des;
            }
            catch
            {
                Description = name;
            }
        }

		public OptionAttribute(string name, Type type, Type mainOption, int index)
		{
			Name = name;
			var res = new ResourceManager(type);
			try
			{
				string des = res.GetString("Meta.Description");
				if(!string.IsNullOrEmpty(des))
					Description = des;
			}
			catch
			{
				Description = name;
			}
			MainOption = mainOption;
			Index = index;
		}

        public string Name { get; private set; }

        public string Description { get; set; }

		public Type MainOption { get; private set; }

		public int Index { get; private set; }
    }
}