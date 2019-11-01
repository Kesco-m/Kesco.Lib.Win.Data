using System.Data;

namespace Kesco.Lib.Win.Data.Business.V2
{
    public interface IReaderListener
    {
        void OnRead(IDataReader reader);
    }
}
