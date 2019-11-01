using System.Data;

namespace Kesco.Lib.Win.Data.Business.V2
{
    public class PagedReaderListener : ReaderListener
    {
        private int pageSize;
        private int pageIndex;

        public override void OnRead(IDataReader r)
        {
            if (Index >= pageIndex * pageSize && Index < (pageIndex + 1) * pageSize) Process(r);
            Index++;
        }

        public PagedReaderListener(int pageSize, int pageIndex)
        {
            this.pageSize = pageSize;
            this.pageIndex = pageIndex;
        }
    }
}
