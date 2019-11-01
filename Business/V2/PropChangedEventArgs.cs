using System;

namespace Kesco.Lib.Win.Data.Business.V2
{
    public class PropChangedEventArgs : EventArgs
    {
        private string propName;
        private string oldValue;
        private string newValue;

        public string PropName
        {
            get { return propName; }
        }

        public string OldValue
        {
            get { return oldValue; }
        }

        public string NewValue
        {
            get { return newValue; }
        }

        public PropChangedEventArgs(string propName, string oldValue, string newValue)
        {
            this.propName = propName;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }
}
