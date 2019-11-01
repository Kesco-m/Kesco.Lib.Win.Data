using System.Data;

namespace Kesco.Lib.Win.Data.Types
{
    /// <summary>
    /// Summary description for Type.
    /// </summary>
    public abstract class KType
    {
        protected bool isUndefined = true;

        public bool IsUndefined
        {
            get { return isUndefined; }
        }

        public string AttributeName { get; set; }

        public string SqlParameterName { get; set; }

        public SqlDbType SqlParameterDBType { get; set; }

        public string SqlColumnName { get; set; }

        public abstract string Xml { get; set; }

        public abstract void Fill(DataRow row);

        public KType(string sqlColumnName, string sqlParameterName, SqlDbType sqlParameterDBType, string attributeName)
        {
            SqlColumnName = sqlColumnName;
            SqlParameterName = sqlParameterName;
            SqlParameterDBType = sqlParameterDBType;
            AttributeName = attributeName;
        }
    }
}
