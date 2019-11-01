using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    /// <summary>
    /// DAL-компонент для доступа к хранимым процедурам по построению дерева каталога документов.
    /// </summary>
    public class DocTreeSPDALC : DALC
    {
        private const string sp_TreeSubNodes = "sp_TreeSubNodes";

        private const string keyField = "Key";
        private const string textField = "Text";
        private const string typeField = "Type";
        private const string lvlField = "Lvl";
        private const string subNodesField = "SubNodes";

        public DocTreeSPDALC(string connectionString)
            : base(connectionString)
        {
        }

        #region Accessors

        public string KeyField
        {
            get { return keyField; }
        }

        public string TextField
        {
            get { return textField; }
        }

        public string TypeField
        {
            get { return typeField; }
        }

        public string LvlField
        {
            get { return lvlField; }
        }

        public string SubNodesField
        {
            get { return subNodesField; }
        }

        #endregion

        #region Get Data

        public DataTable GetTreeSubNodes(string path)
        {
            return GetDataTable(sp_TreeSubNodes,
                                delegate(SqlCommand cmd)
                                    {
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        AddParam(cmd, "@Path", SqlDbType.VarChar, path);
                                    });
        }

        #endregion
    }
}