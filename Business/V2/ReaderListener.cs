using System.Data;

namespace Kesco.Lib.Win.Data.Business.V2
{
    public class ReaderListener : IReaderListener
    {
        public int Index { get; protected set; }

        public virtual void OnRead(IDataReader r)
        {
            Process(r);
            Index++;
        }

        public virtual void Process(IDataReader r)
        {
            //Dosomething
        }

        public ReaderListener()
        {
            Index = 0;
        }
    }
}
