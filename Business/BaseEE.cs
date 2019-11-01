using System;
using System.Data;
using System.Xml;
using Kesco.Lib.Win.Data.Business.Corporate;

namespace Kesco.Lib.Win.Data.Business
{
    public abstract class BaseEE : Base
    {
        private Employee editor;
        private DateTime edited;

        #region Accessors

        /// <summary>
        /// Изменил
        /// </summary>
        public Employee Editor
        {
            get
            {
                LoadIfDelayed();
                return editor;
            }
            set { editor = value; }
        }

        /// <summary>
        /// Изменено
        /// </summary>
        public DateTime Edited
        {
            get
            {
                LoadIfDelayed();
                return edited;
            }
            set { edited = value; }
        }

        #endregion

        #region TEXT REPRESENTETION

        public string EditorID
        {
            get { return Editor != null ? Editor.ID.ToString() : ""; }
        }

        public string EditorName
        {
            get { return Editor != null ? Editor.Name : ""; }
        }

        public string EditedText
        {
            get { return Edited == DateTime.MinValue ? "" : Edited.ToString(); }
        }

        #endregion

        #region Fields

        /// <summary>
        /// название поля "Изменил" (уже проинициализированно)
        /// </summary>
        protected virtual string Editor_Field
        {
            get { return "Изменил"; }
        }

        /// <summary>
        /// название поля "Изменено" (уже проинициализированно)
        /// </summary>
        protected virtual string Edited_Field
        {
            get { return "Изменено"; }
        }

        #endregion

        #region Constructors

        public BaseEE()
        {
        }

        public BaseEE(int id) : base(id)
        {
        }

        #endregion

        #region XML

        public override void LoadFromXmlElement(XmlElement el)
        {
            base.LoadFromXmlElement(el);

            string atr;

            atr = el.GetAttribute("EditorID");
            editor = atr.Length > 0 ? new Employee(int.Parse(atr)) : null;

            atr = el.GetAttribute("Edited");
            edited = atr.Length > 0 ? DateTime.Parse(atr) : DateTime.MinValue;

        }

        #endregion

        #region DataBase

        protected override void Fill(DataRow row)
        {
            base.Fill(row);
            editor = row.IsNull(Editor_Field) ? null : new Employee((int) row[Editor_Field]);
            edited = row.IsNull(Edited_Field) ? DateTime.MinValue : (DateTime) row[Edited_Field];
        }

        #endregion
    }
}