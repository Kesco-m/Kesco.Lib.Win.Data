using System;

namespace Kesco.Lib.Win.Data.Business
{
    public class BeforeChangeEventArgs : EventArgs
    {
        private string propertyName;
        private object oldValue;
        private object newValue;

        #region ACCESSORS

        public string PropertyName
        {
            get { return propertyName; }
        }

        public object OldValue
        {
            get { return oldValue; }
        }

        public object NewValue
        {
            get { return newValue; }
        }

        public bool Cancel { get; set; }

        #endregion

        public BeforeChangeEventArgs(string propertyName, object oldValue, object newValue)
        {
            Cancel = false;
            this.propertyName = (propertyName ?? "");
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }
}
