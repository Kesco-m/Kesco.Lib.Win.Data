using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Kesco.Lib.Win.Data.Business.V2.FilterOptions;

namespace Kesco.Lib.Win.Data.Business.V2
{
	public abstract class Dso
	{
		DBModule module;

	    public bool notUseSearchTextOption;

		internal ArrayList opts = new ArrayList();
		internal Hashtable optsIDs = new Hashtable();	//содержит ссылки на пользовательские опции

		public TextWriter w;
		public SqlCommand cm;
		internal ArrayList opts2render = new ArrayList();

		public virtual int GetID(DataRow row)
		{
			return 0;
		}

		public virtual string GetName(DataRow row)
		{
			return "#" + GetID(row);
		}

	    public int StartRecord { get; set; }

	    public int MaxRecords { get; set; }

	    protected void PopulateOptsIDs()
		{
			var ids = new string[10];
			int n;
			foreach (FOpt opt in opts.Cast<FOpt>().Where(opt => opt.Is4User))
			{
			    n = 0;
			    opt.GetIDs(ids, ref n);
			    for (int i = 0; i < n; i++)
			        optsIDs[ids[i].ToLower()] = opt;
			}
		}

		public void ExecuteReader(IReaderListener listener)
		{
			module.ExecuteReader(CmdText, listener);
		}

		#region SQL Command Rendering

		public virtual SqlCommand GetSqlCommand()
		{
		    cm = new SqlCommand {CommandText = CmdText};
		    return cm;
		}

		public virtual void RenderCmdText()
		{
			RenderSqlPrepClause();
			RenderSqlSelectClause();
			RenderSqlWhereClause(); // cm, передается для заполнения параметров
			RenderSqlOrderByClause();
			RenderSqlHintsClause();
		}

		public string CmdText
		{
			get
			{
				w = new StringWriter();
				RenderCmdText();
				return w.ToString();
			}
		}

		public virtual SqlCommand GetSelectByPKCommand(string whereClause)
		{
			var w = new StringWriter();
			RenderSqlSelectClause();
			w.Write(" WHERE " + whereClause);
		    var cm = new SqlCommand {CommandText = w.ToString()};
		    return cm;
		}

		protected virtual void RenderSqlPrepClause()
		{
			opts2render.Clear();
			foreach (FOpt opt in opts)
				if (opt.Is4Sql && opt.RenderSqlPrepClause())
					opts2render.Add(opt);
		}

		protected abstract void RenderSqlSelectClause();

		protected virtual void RenderSqlWhereClause()
		{
			int n = opts2render.Count;

			var b = new StringBuilder();
			var sw = new StringWriter(b);

			RenderSqlWhereClauseEx(sw);

			if (n == 0 && b.Length == 0)
                return;

			w.Write("\r\nWHERE ");

			if (b.Length > 0)
                w.Write(" {0}", b);
			if (n > 0 && b.Length > 0)
                w.Write(" AND ");

			FOpt opt;
			for (int i = 0; i < n; i++)
			{
				opt = (FOpt)opts2render[i];
				if (n > 1 && i > 0) 
                    w.Write("\r\nAND ");
				if (n > 1)
                    w.Write("(");
				opt.RenderSqlWhereClause();
				if (n > 1)
                    w.Write(")");
			}
		}

		public virtual void RenderSqlWhereClauseEx(StringWriter w)
		{
		}

		protected virtual void RenderSqlOrderByClause()
		{
		}

		protected virtual void RenderSqlHintsClause()
		{
		}

		#endregion

		/// <summary>
		/// если опции указаны, то все они обязательны
		/// </summary>
		public bool HasEnabledNotFixedOptions()
		{
		    return opts.Cast<FOpt>().Any(opt => opt.Enabled && !opt.Fixed && !opt.ID.ToLower().Equals("search"));
		}

	    public Dso(DBModule module)
		{
		    MaxRecords = -1;
		    this.module = module;
		}
	}
}