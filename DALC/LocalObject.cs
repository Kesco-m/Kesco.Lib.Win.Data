using System;

namespace Kesco.Lib.Win.Data.DALC
{
    /// <summary>
    /// Summary description for LocalObject.
    /// </summary>
    public class LocalObject
    {
        public static TimeSpan GetTimeDiff()
        {
            DateTime curDate = DateTime.Now;
            return curDate - curDate.ToUniversalTime();
        }
    }
}
