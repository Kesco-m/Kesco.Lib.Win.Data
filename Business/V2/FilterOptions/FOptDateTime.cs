using System;
using System.Text.RegularExpressions;

namespace Kesco.Lib.Win.Data.Business.V2.FilterOptions
{
	public class FOptDateTime : FOpt
	{
		public override bool RenderSqlPrepClause()
		{
			return base.RenderSqlPrepClause() && items.Count > 0;
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
				ds.w.Write(Regex.Replace(
					curItem.value,
					"^(\\d{4,4})(\\d{2,2})(\\d{2,2})(\\d{2,2})(\\d{2,2})(\\d{2,2})$",
					"'$1$2$3 $4:$5:$6'"));
			}
		}

		public void Set(DateTime value) { Set(value, itemFlagsDefault); }
		public void Set(DateTime value, FOptItemFlags flags) { Set(value.ToString("yyyyMMddHHmmss"), flags); }
		public void Add(DateTime value) { Add(value, itemFlagsDefault); }
		public void Add(DateTime value, FOptItemFlags flags) { Add(value.ToString("yyyyMMddHHmmss"), flags, true); }

		protected override void Add(string value, FOptItemFlags flags, bool throwOnError)
		{
			if (!AssignItemsFlags(ref flags, throwOnError)) return;

			if ((flags & FOptItemFlags.IsNull) == FOptItemFlags.IsNull)
			{
				Add("", flags);
				return;
			}


			Match m = Regex.Match(value, "^(\\d{4,4})?(\\d{2,2})?(\\d{2,2})?(\\d{2,2})?(\\d{2,2})?(\\d{2,2})?$");
			if (!m.Success)
			{
				if (throwOnError)
                    throw new Exception("Некорректно указано значение элемнта опции поиска '" + ID + "'");
				return;
			}

			int y = m.Groups[1].Success ? int.Parse(m.Groups[1].Value) : 0;
			int MM = m.Groups[2].Success ? int.Parse(m.Groups[2].Value) : 0;
			int d = m.Groups[3].Success ? int.Parse(m.Groups[3].Value) : 0;
			int hh = m.Groups[4].Success ? int.Parse(m.Groups[4].Value) : 0;
			int mm = m.Groups[5].Success ? int.Parse(m.Groups[5].Value) : 0;
			int ss = m.Groups[6].Success ? int.Parse(m.Groups[6].Value) : 0;

			try
			{
				var dt = new DateTime(y, MM, d, hh, mm, ss);
				Add(dt.ToString("yyyyMMddHHmmss"), flags);
			}
			catch
			{
				if (throwOnError) throw new Exception("Некорректно указано значение элемнта опции поиска '" + ID + "'");
			}
		}

		public FOptDateTime(Dso ds, string id)
			: base(ds, id)
		{
			flags = FOptFlags.None;
			flagsMask = FOptFlags.Enabled | FOptFlags.Fixed | FOptFlags.Inverse | FOptFlags.MatchAnyItem;
			itemFlagsDefault = FOptItemFlags.Equals;
			itemFlagsMask = FOptItemFlags.IsNull | FOptItemFlags.Equals | FOptItemFlags.Less | FOptItemFlags.More;
		}
	}
}
