using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace Kesco.Lib.Win.Data.DALC
{
    /// <summary>
    /// Базовый класс DAL-компонента для дерева
    /// </summary>
    public class TreeDALC : DALC
    {
        protected string parentField = "Parent";
        protected string leftField = "L";
        protected string rightField = "R";

        protected string parentRelation = "ParentRelation";

        public TreeDALC(string connectionString) : base(connectionString)
        {
        }

        #region Accessors

        public string LeftField
        {
            get { return leftField; }
        }

        public string RightField
        {
            get { return rightField; }
        }

        public string ParentField
        {
            get { return parentField; }
        }

        public string ParentRelation
        {
            get { return parentRelation; }
        }

        #endregion

        protected void AddParentRelation(DataSet ds, CancellationToken ct)
        {
            DataColumn pk, fk;
            pk = ds.Tables[tableName].Columns[idField];
            fk = ds.Tables[tableName].Columns[parentField];
			if(ct != CancellationToken.None && ct.IsCancellationRequested)
				ct.ThrowIfCancellationRequested();
            ds.Relations.Add(new DataRelation(parentRelation, pk, fk, false));
        }

		/// <summary>
		/// Получение данных дерева
		/// </summary>
		/// <param name="query">запрос к базе</param>
		/// <param name="addParams">заполенние параметров запроса</param>
		/// <returns></returns>
		internal protected DataSet GetTreeData(string query, AddParams addParams)
		{
			return GetTreeData(query, addParams, CancellationToken.None);
		}

		internal protected DataSet GetTreeData(string query, AddParams addParams, CancellationToken ct)
		{
			using(var sda = new SqlDataAdapter())
			using(var conn = new SqlConnection(connectionString))
			using(sda.SelectCommand = new SqlCommand(query, conn))
			{
				if(ct != CancellationToken.None)
					ct.Register(() => sda.SelectCommand.Cancel());
				if(addParams != null)
					addParams(sda.SelectCommand);
				if(ct != CancellationToken.None && ct.IsCancellationRequested)
					ct.ThrowIfCancellationRequested();
				DataSet ds = CMD_FillDS(sda);
				if(ds != null)
					AddParentRelation(ds, ct);
				if(ct != CancellationToken.None && ct.IsCancellationRequested)
					ct.ThrowIfCancellationRequested();
				return ds;
			}
		}
    }
}