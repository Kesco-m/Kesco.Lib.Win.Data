using System;
using System.Collections;
using System.Data.SqlClient;
using System.Xml;

namespace Kesco.Lib.Win.Data.Business
{
    public class BaseCollection : CollectionBase
    {
        protected Type type;

        public Base this[int index]
        {
            get { return ((Base) List[index]); }
            set { List[index] = value; }
        }

        public int Add(Base value)
        {
            return (List.Add(value));
        }

        public int IndexOf(Base value)
        {
            return (List.IndexOf(value));
        }

        public void Insert(int index, Base value)
        {
            List.Insert(index, value);
        }

        public void Remove(Base value)
        {
            List.Remove(value);
        }

        public bool Contains(Base value)
        {
            // If value is not of type Int16, this will return false.
            return (List.Contains(value));
        }

        public bool Contains2(Base value)
        {
            for (int i = 0; i < Count; i++)
                if (this[i].ID == value.ID)
                    return true;
            return false;
        }

        protected override void OnInsert(int index, Object value)
        {
            if (value.GetType() != type)
                throw new ArgumentException("value must inherit from type " + type.FullName, "value");
        }

        protected override void OnRemove(int index, Object value)
        {
            if (value.GetType() != type)
                throw new ArgumentException("value must inherit from type " + type.FullName, "value");
        }

        protected override void OnSet(int index, Object oldValue, Object newValue)
        {
            if (newValue.GetType() != type)
                throw new ArgumentException("newValue must inherit from type " + type.FullName, "newValue");
        }

        protected override void OnValidate(Object value)
        {
            if (value.GetType() != type)
                throw new ArgumentException("value must inherit from type " + type.FullName);
        }


        public BaseCollection()
        {
            type = typeof (Base);
        }

        #region XML

        public virtual void LoadFromXmlElement(XmlElement el)
        {
            Clear();
            Base obj;

            for (int i = 0; i < el.ChildNodes.Count; i++)
            {
                obj = (Base) Activator.CreateInstance(type);
                obj.LoadFromXmlElement((XmlElement) el.ChildNodes[i]);
                Add(obj);
            }
        }

        public virtual void SaveToXmlElement(XmlElement el)
        {
            XmlElement el2;
            for (int i = 0; i < Count; i++)
            {
                el2 = el.OwnerDocument.CreateElement(this[i].GetType().Name);
                this[i].SaveToXmlElement(el2);
                el.AppendChild(el2);
            }
        }

        #endregion

        #region DB SAVING

        public virtual void DB_Update(BaseCollection col0, SqlTransaction tran)
        {
            if (col0 != null)
                for (int i = 0; i < col0.Count; i++)
                    if (!Contains2(col0[i])) col0[i].DB_Delete(tran);

            for (int i = 0; i < Count; i++)
                if (col0 != null && col0.Contains2(this[i])) this[i].DB_Update(null, tran);
                else this[i].DB_Insert(tran);
        }

        #endregion
    }
}
