using System;
using System.Text.RegularExpressions;

namespace Kesco.Lib.Win.Data.Business.V2.FilterOptions
{
	public class FOptInt : FOpt
	{
		protected void RenderCSVList()
		{
			int n = 0;
			foreach (FOptItem item in items)
			{
				if (item.IsNull) continue;
				if (n > 0) ds.w.Write(",");
				ds.w.Write(item.value);
				n++;
			}
		}

		public override bool RenderSqlPrepClause()
		{
			if (!base.RenderSqlPrepClause()) return false;

			return items.Count > 0;
		}

		protected override void RenderSqlWhereClauseItem()
		{
			RenderSqlLValue();
			if ((curItem.flags & FOptItemFlags.IsNull) == FOptItemFlags.IsNull)
			{
				ds.w.Write(" IS NULL");
			}
			else
			{
				RenderLessEqualsMore();
				ds.w.Write(curItem.value);
			}
		}

		public void Set(int value) { Set(value, itemFlagsDefault); }
		public void Set(int value, FOptItemFlags flags) { Set(value.ToString(), flags); }
		public void Add(int value) { Add(value, itemFlagsDefault); }
		public void Add(int value, FOptItemFlags flags) { Add(value.ToString(), flags, true); }

		protected override void Add(string value, FOptItemFlags flags, bool throwOnError)
		{
			if (!AssignItemsFlags(ref flags, throwOnError))
                return;

			if ((flags & FOptItemFlags.IsNull) == FOptItemFlags.IsNull)
			{
				Add("", flags);
				return;
			}

			Match m = Regex.Match(value, "^(-?)(\\d+)$");
			if (!m.Success)
			{
			    if (throwOnError) 
                    throw new Exception("Некорректно указано значение элемнта опции поиска '" + ID + "'");
			    return;
			}

		    Add(m.Value, flags);
		}

		protected virtual bool IsFixed(string url)
		{
			return Fixed;
		}

		public FOptInt(Dso ds, string id)
			: base(ds, id)
		{
			flags = FOptFlags.None;
			flagsMask = FOptFlags.Enabled | FOptFlags.Fixed | FOptFlags.Inverse | FOptFlags.MatchAnyItem;
			itemFlagsDefault = FOptItemFlags.Equals;
			itemFlagsMask = FOptItemFlags.IsNull | FOptItemFlags.Equals | FOptItemFlags.Less | FOptItemFlags.More;
		}
	}
}