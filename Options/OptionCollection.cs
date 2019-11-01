using System;
using System.Collections;

namespace Kesco.Lib.Win.Data.Options
{
    public class OptionCollection : CollectionBase
    {
        private static Type t = typeof (Option);

        public Option this[int index]
        {
            get { return ((Option) List[index]); }
            set { List[index] = value; }
        }

        public int Add(Option value)
        {
            return (List.Add(value));
        }

        public int IndexOf(Option value)
        {
            return (List.IndexOf(value));
        }

        public void Insert(int index, Option value)
        {
            List.Insert(index, value);
        }

        public void Remove(Option value)
        {
            List.Remove(value);
        }

        /// <summary>
        /// Checks whether specific value can be found in options
        /// </summary>
        /// <returns>If value is not of type Int16, this will return false</returns>
        public bool Contains(Option value)
        {
            return (List.Contains(value));
        }

        protected override void OnInsert(int index, Object value)
        {
            if (!(value.GetType() == t || value.GetType().IsSubclassOf(t)))
                throw new ArgumentException("value must inherit from type " + t.FullName, "value");
        }

        protected override void OnRemove(int index, Object value)
        {
            if (!(value.GetType() == t || value.GetType().IsSubclassOf(t)))
                throw new ArgumentException("value must inherit from type " + t.FullName, "value");
        }

        protected override void OnSet(int index, Object oldValue, Object newValue)
        {
            if (!(newValue.GetType() == t || newValue.GetType().IsSubclassOf(t)))
                throw new ArgumentException("newValue must inherit from type " + t.FullName, "newValue");
        }

        protected override void OnValidate(Object value)
        {
            if (!(value.GetType() == t || value.GetType().IsSubclassOf(t)))
                throw new ArgumentException("value must inherit from type " + t.FullName);
        }
    }
}
