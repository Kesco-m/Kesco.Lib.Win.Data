using System;
using System.Collections;
using System.Xml;

namespace Kesco.Lib.Win.Data.Business.V2.FilterOptions
{
	public abstract class FOpt
	{
		protected Dso ds;

	    readonly string id;
		protected string description;
		int order;
		protected bool is4User;
		protected bool is4Sql;

		protected FOptFlags flags;					//общие для опции флаги (в конструкторе устанавливаются флаги поумолчанию)
		protected FOptFlags flagsMask;				//маска флагов опции, которые может редактировать пользователь
		protected FOptItemFlags itemFlagsDefault;	//default значение флагов элемента опции
		protected FOptItemFlags itemFlagsMask;		//маска флагов элемента опции, которые может редактировать пользователь

		protected ArrayList items = new ArrayList();

		protected FOptItem curItem;	//Текущий элемент опции

		#region ACCESSORS
		/// <summary>
		/// идентификатор опции используется при генерации SQL запросов и для сохранения в XML
		/// </summary>
		public string ID
		{
			get { return id; }
		}

		public int Order
		{
			get { return order; }
			set { order = value; }
		}
		/// <summary>
		/// краткое описание опции (используется в списках опций для выбора необходимой)
		/// </summary>
		public string Description
		{
			get { return description; }
		}
		public bool Is4User
		{
			get { return is4User; }
		}
		public bool Is4Sql
		{
			get { return is4Sql; }
		}

		//ФЛАГИ
		public FOptItemFlags ItemFlagsDefault
		{
			get { return itemFlagsDefault; }
			set
			{
				FOptItemFlags changes = itemFlagsDefault ^ value;
				FOptItemFlags validChanges = changes & itemFlagsMask;
				FOptItemFlags invalidChanges = (changes | itemFlagsMask) ^ itemFlagsMask;

				if (invalidChanges != FOptItemFlags.None)
					throw new Exception("Устанавливаются или сбрасываются недопустимые флаги: " + invalidChanges.ToString() + "\nопция " + ID + ", свойство ItemFlagsDefault.");

				itemFlagsDefault ^= validChanges;	//применяем изменения проходящие через маску
			}
		}

		public FOptFlags Flags
		{
			get { return flags; }
			set
			{
				FOptFlags changes = flags ^ value;
				FOptFlags validChanges = changes & flagsMask;
				FOptFlags invalidChanges = (changes | flagsMask) ^ flagsMask;

				if (invalidChanges != FOptFlags.None)
					throw new Exception("Устанавливаются или сбрасываются недопустимые флаги: " + invalidChanges.ToString() + "\nопция " + ID + ", свойство Flags.");

				flags ^= validChanges;	//применяем изменения проходящие через маску
			}
		}

		/// <summary>
		/// говорит о том, что опция зафиксированна и не может быть изменена
		/// </summary>
		public bool Fixed
		{
			get
			{
				return (flags & FOptFlags.Fixed) == FOptFlags.Fixed;
			}
			set
			{
				if (value) Flags |= FOptFlags.Fixed;
				else if (Fixed) Flags ^= FOptFlags.Fixed;
			}
		}

		public bool Enabled
		{
			get
			{
				return (flags & FOptFlags.Enabled) == FOptFlags.Enabled;
			}
			set
			{
				if (value)
                    Flags |= FOptFlags.Enabled;
				else if (Enabled)
                    Flags ^= FOptFlags.Enabled;
			}
		}
		public bool Inverse
		{
			get
			{
				return (flags & FOptFlags.Inverse) == FOptFlags.Inverse;
			}
			set
			{
				if (value)
                    Flags |= FOptFlags.Inverse;
				else if (Inverse)
                    Flags ^= FOptFlags.Inverse;
			}
		}

		public bool MatchAnyItem
		{
			get
			{
				return (flags & FOptFlags.MatchAnyItem) == FOptFlags.MatchAnyItem;
			}
			set
			{
				if (value)
                    Flags |= FOptFlags.MatchAnyItem;
				else if (MatchAnyItem)
                    Flags ^= FOptFlags.MatchAnyItem;
			}
		}

		#endregion

		internal virtual void GetIDs(string[] ids, ref int i)
		{
			ids[i++] = id;
		}

		#region SQL

		public virtual bool RenderSqlPrepClause()
		{
			return Enabled;
		}

		public virtual void RenderSqlWhereClause()
		{
			if (Inverse) ds.w.Write(" NOT (");
			RenderSqlWhereClauseItems();
			if (Inverse) ds.w.Write(")");
		}

		protected void RenderSqlWhereClauseItems()
		{
			int n = items.Count;
			for (int i = 0; i < n; i++)
			{
				curItem = (FOptItem)items[i];

				if (i > 0) { ds.w.Write(MatchAnyItem ? " OR " : " AND "); }
				if (n > 1) ds.w.Write("(");

				RenderSqlWhereClauseItem();

				if (n > 1) ds.w.Write(")");
			}
		}

		protected virtual void RenderSqlWhereClauseItem()
		{
			RenderSqlLValue();
			RenderLessEqualsMore();
			RenderSqlRValue();
		}

		protected virtual void RenderSqlLValue()
		{

		}

		protected virtual void RenderSqlRValue()
		{

		}

		protected void RenderLessEqualsMore()
		{
			bool l = (curItem.flags & FOptItemFlags.Less) == FOptItemFlags.Less;
			bool m = (curItem.flags & FOptItemFlags.More) == FOptItemFlags.More;
			bool e = (curItem.flags & FOptItemFlags.Equals) == FOptItemFlags.Equals;

			if ((l & m & e) || (!l & !m & !e)) throw new Exception("Ошибочно установлены флаги элемента опции поиска '" + ID + "' - " + flags.ToString());

			if (l) ds.w.Write("<");
			if (m) ds.w.Write(">");
			if (e) ds.w.Write("=");
		}

		#endregion

		#region XML

		public virtual void SaveToXmlElement(XmlElement el)
		{
		    el.SetAttribute("id", ID);
		    el.SetAttribute("flags", ((int) flags).ToString());
		    el.SetAttribute("order", order.ToString());

		    XmlElement elIt;
		    foreach (FOptItem item in items)
		        if (el.OwnerDocument != null)
		        {
		            el.AppendChild(elIt = el.OwnerDocument.CreateElement("Item"));
		            elIt.SetAttribute("value", item.value);
		            elIt.SetAttribute("flags", ((int) item.flags).ToString());
		        }
		}

		public virtual void LoadFromXmlElement(XmlElement el)
		{
			flags = (FOptFlags)int.Parse(el.GetAttribute("flags"));

			items.Clear();
            if (el.SelectNodes("Item") != null)
            {
                foreach (XmlElement elIt in el.SelectNodes("Item"))
                    Add(elIt.GetAttribute("value"),
                        (FOptItemFlags) int.Parse(elIt.GetAttribute("flags")),
                        false);
            }
		}

		#endregion

		public int Count
		{
			get { return items.Count; }
		}

		public string GetItemValue(int index)
		{
		    return index >= items.Count ? null : ((FOptItem)items[index]).value;
		}

	    public void Clear()
		{
			items.Clear();
		}

		protected void Set(string value, FOptItemFlags flags)
		{
			items.Clear();
			Add(value, flags, true);
		}

		protected virtual void Add(string value, FOptItemFlags flags, bool throwOnError)
		{

		}

		public virtual void Add(string value, FOptItemFlags flags)
		{
			FOptItem item;
			for (int i = items.Count - 1; i >= 0; i--)
			{
				item = (FOptItem)items[i];
				if (item.value.Equals(value))
					items.RemoveAt(i);
			}
			items.Add(new FOptItem(value, flags));
		}

		protected bool AssignItemsFlags(ref FOptItemFlags flags, bool throwOnError)
		{
			FOptItemFlags changes = itemFlagsDefault ^ flags;
			FOptItemFlags validChanges = changes & itemFlagsMask;
			FOptItemFlags invalidChanges = (changes | itemFlagsMask) ^ itemFlagsMask;

			if (throwOnError && (invalidChanges != FOptItemFlags.None)) //проверяем флаги, которые устанавливаются или сбрасываются
				throw new Exception("При добавлении элемента в опцию " + ID + " устанавливаются или сбрасываются недопустимые флаги: " + invalidChanges.ToString() + ".");

			flags = itemFlagsDefault ^ validChanges;	//применяем пользовательские изменения к itemFlagsDefault

		    if ((flags & (FOptItemFlags.Less | FOptItemFlags.EqualsOrMore)) ==
		        (FOptItemFlags.Less | FOptItemFlags.EqualsOrMore))
		        throw new Exception("Недопустимо одновременное применение флагов: Less,More и Equals\nДобавление элемента в опцию " + ID +" .");

		    if (flags == FOptItemFlags.None)
		        throw new Exception("Необходимо указать хотя бы один флаг.\nДобавление элемента в опцию " + ID + " .");

		    return true;
		}

		public FOpt(Dso ds, string id)
		{
			this.ds = ds;
			flags = FOptFlags.None;

			this.id = id;

			description = "";
			is4Sql = true;
			is4User = true;

			order = ds.opts.Count;
			ds.opts.Add(this);
		}
	}
}
