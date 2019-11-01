
#if !CLEAR
namespace Business.V2
{
	/// <summary>
	/// Summary description for Base.
	/// </summary>
	public abstract class Base
	{
		protected V2.Context context;
		int id;
		DataRow row;

		protected abstract void AttachRowInContext(ref DataRow row);
		
		public bool Unavailable
		{
			get
			{
				try 
				{
					if (row==null) AttachRowInContext(ref row);
					return false;
				}
				catch(Exception ex)
				{
					return true;
				}
			}
		}
		public int ID
		{
			get{return id;}
		}
		
		public DataRow Row
		{
			get
			{
				if (row==null) AttachRowInContext(ref row);
				return row;
			}
		}

		public object this[string columnName]
		{
			get{return Row[columnName];}
		}

		public int GetModifiedItemsNumber()
		{
			if(Row.RowState==DataRowState.Added) return row.Table.Columns.Count;
			int n=0;
			foreach(DataColumn cl in row.Table.Columns)
				if(!Row[cl.ColumnName,DataRowVersion.Original].Equals(Row[cl.ColumnName])) n++;
			return n;
		}

		public Base(V2.Context context, DataRow row)
		{
			this.context=context;
			this.row=row;
		}
		public Base(V2.Context context, int id)
		{
			this.context=context;
			this.id=id;
		}
	}
}
#endif