namespace Kesco.Lib.Win.Data.Temp.Objects
{
    /// <summary>
    /// Summary description for IDObject.
    /// </summary>
    public class IDObject
    {
        protected int id;

        public IDObject(int id)
        {
            this.id = id;
        }

        #region Accessors

        public int ID
        {
            get { return id; }
        }

        #endregion
    }
}