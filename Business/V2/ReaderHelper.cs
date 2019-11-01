using System;
using System.Data;

namespace Kesco.Lib.Win.Data.Business.V2
{
    public class ReaderHelper : IDisposable
    {
        private IDataReader reader;

        public IDataReader Reader
        {
            get { return reader; }
        }

        public DateTime GetDateTime(string name)
        {
            int index = reader.GetOrdinal(name); //эта операция затратна, надо как-то изменить
            return reader.IsDBNull(index) ? DateTime.MaxValue : reader.GetDateTime(index);
        }

        public int GetInt32(string name)
        {
            int index = reader.GetOrdinal(name);
            return reader.IsDBNull(index) ? int.MaxValue : reader.GetInt32(index);
        }

        public string GetString(string name)
        {
            int index = reader.GetOrdinal(name);
            return reader.IsDBNull(index) ? string.Empty : reader.GetString(index);
        }

        public decimal GetDecimal(string name)
        {
            int index = reader.GetOrdinal(name);
            return reader.IsDBNull(index) ? decimal.MaxValue : reader.GetDecimal(index);
        }

        public ReaderHelper(IDataReader reader)
        {
            this.reader = reader;
        }

        #region IDisposable Members

        public void Dispose()
        {
            reader.Dispose();
        }

        #endregion
    }
}
