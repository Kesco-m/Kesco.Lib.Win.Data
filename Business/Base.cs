using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Kesco.Lib.Win.Data.Business
{
    public delegate void ChangedDelegate();

    public delegate void BeforeChangeEventHandler(object sender, BeforeChangeEventArgs e);

    public abstract class Base
    {
        private XmlElement elCache; //xml элемент из которого был загружен данный объект
        protected string connectionString;
        protected int id;
        protected States state;

        #region Accessors

        public virtual bool IsNew
        {
            get { return ID == 0; }
        }

        public int ID
        {
            get { return id; }
        }

        public bool IsUnavailable
        {
            get
            {
                LoadIfDelayed();
                return (state == States.LoadFailed);
            }
        }

        /// <summary>
        /// дата актуальности объекта (например, для выбора карточки лица)
        /// </summary>
        public virtual DateTime CurrentDate
        {
            get { return DateTime.Now; }
        }

        #endregion

        #region Fields

        protected abstract string ID_Field { get; }
        protected abstract string FillFrom_Table { get; }

        #endregion

        public static bool operator ==(Base obj1, Base obj2)
        {
            bool b = ((object) obj1 == null && (object) obj2 == null) ||
                     ((object) obj1 != null && (object) obj2 != null && obj1.ID == obj2.ID);
            return b;
        }

        public static bool operator !=(Base obj1, Base obj2)
        {
            return !(obj1 == obj2);
        }

        public static int[] GetIDsDifferenceIntArray(string s0, string s1)
        {
            string[] SArr = GetIDsDifference(s0, s1).Split(',');
            if (SArr.Length == 1 && SArr[0].Equals("")) return new int[0];
            var IArr = new int[SArr.Length];
            for (int i = 0; i < SArr.Length; i++) IArr[i] = int.Parse(SArr[i]);
            return IArr;
        }

        public static string GetIDsDifference(string s0, string s1)
        {
            string ret =
                s0.Split(',').Where(x => Regex.IsMatch(x, "^[\\d]+$")).Where(
                    x => !Regex.IsMatch(s1, "(^|,)" + x + "(,|$)")).Aggregate("", (current, x) => current + (x + ","));
            return Regex.Replace(ret, ",$", "");
        }

        #region Constructors

        public Base()
        {
            id = 0;
            state = States.New;
            Init();
        }

        public Base(int id)
        {
            this.id = id;
            state = States.LoadDelayed;
            Init();
        }

        protected virtual void Init()
        {
            connectionString = Settings.DS_document;
        }

        #endregion

        #region XML

        public virtual string RootElementName
        {
            get { return GetType().ToString(); }
        }

        public bool SetAttribute(XmlElement el, string name, string value)
        {
            el.SetAttribute(name, value);
            return elCache == null || !value.Equals(elCache.GetAttribute(name));
        }

        public void SetAttributeFromCache(XmlElement el, string name)
        {
            el.SetAttribute(name, elCache.GetAttribute(name));
        }

        public void ClearCache()
        {
            elCache = null;
        }

        public virtual void SaveToXmlElement(XmlElement el)
        {
            el.SetAttribute("ID", ID.ToString());
        }

        public virtual void LoadFromXmlElement(XmlElement el)
        {
            elCache = el;
            state = States.Loaded;

            string atr = el.GetAttribute("ID");
            id = atr.Length > 0 ? int.Parse(atr) : 0;
        }


        public void LoadFromXml(string xml)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            LoadFromXmlElement(xmlDoc.DocumentElement);
        }

        public XmlDocument GetXml()
        {
            var xml = new XmlDocument();
            xml.AppendChild(xml.CreateElement(RootElementName));

            SaveToXmlElement(xml.DocumentElement);

            return xml;
        }

        #endregion

        #region DataBase

        protected void LoadIfDelayed()
        {
            if (state == States.LoadDelayed) Load();
        }

        /// <summary>
        /// Загружает из БД запись по запросу 'SELECT * FROM _Table WHERE ID_Field=ID' 
        /// и вызывает метод Fill для этой записи
        /// </summary>
        protected virtual void Load()
        {
            string query = string.Format("SELECT * FROM {0} WHERE {1}={2}", FillFrom_Table, ID_Field, ID);
            var dt = new DataTable();

            using (var da = new SqlDataAdapter(query, connectionString))
            {
                try
                {
                    da.Fill(dt);
                }
                catch (SqlException sex)
                {
                    Env.WriteSqlToLog(sex, da.SelectCommand);
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex);
                }
            }
            if (dt.Rows.Count != 1) state = States.LoadFailed;
            else Fill(dt.Rows[0]);
        }

        protected virtual void Fill(DataRow row)
        {
            state = States.Loaded;
            id = row.IsNull(ID_Field) ? 0 : (int) row[ID_Field];
        }

        public virtual void Validate()
        {

        }

        public virtual void DB_Save(bool ignoreWarnings)
        {
            Validate();
            using (var cn = new SqlConnection(connectionString))
            {
                SqlTransaction tran = null;

                try
                {
                    cn.Open();
                    tran = cn.BeginTransaction(IsolationLevel.ReadCommitted);

                    if (IsNew) DB_Insert(tran);
                    else DB_Update(null, tran);

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex);
                    if (tran != null) tran.Rollback();
                }
                finally
                {
                    cn.Close();
                }
            }
        }

        public virtual void DB_Insert(SqlTransaction tran)
        {
            using (var cm = new SqlCommand())
            {
                cm.Connection = tran.Connection;
                cm.Transaction = tran;
                SaveToSqlParameters(null, cm.Parameters);
                string cols = "";
                string vals = "";
                foreach (
                    SqlParameter p in
                        from SqlParameter p in cm.Parameters
                        where !ID_Field.Equals(p.ParameterName.Substring(1))
                        select p)
                {
                    cols += (cols.Length > 0 ? "," : "") + p.ParameterName.Substring(1);
                    vals += (vals.Length > 0 ? "," : "") + p.ParameterName;
                }
                cm.CommandText = string.Format("INSERT {0}({1}) VALUES({2})",
                                               new object[] {FillFrom_Table, cols, vals});

                try
                {
                    cm.ExecuteNonQuery();
                }
                catch (SqlException sex)
                {
                    Env.WriteSqlToLog(sex, cm);
                    throw sex;
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex);
                    throw ex;
                }
            }
        }

        public virtual void DB_Update(Base originalObject, SqlTransaction tran)
        {
            using (var cm = new SqlCommand())
            {
                cm.Connection = tran.Connection;
                cm.Transaction = tran;
                SaveToSqlParameters(originalObject, cm.Parameters);
                string upds =
                    (from SqlParameter p in cm.Parameters where !ID_Field.Equals(p.ParameterName.Substring(1)) select p)
                        .Aggregate("",
                                   (current, p) =>
                                   current +
                                   ((current.Length > 0 ? "," : "") + p.ParameterName.Substring(1) + "=" +
                                    p.ParameterName));

                cm.CommandText = string.Format("UPDATE {0} SET {1} WHERE {2}={3}",
                                               new object[] {FillFrom_Table, upds, ID_Field, "@" + ID_Field});

                try
                {
                    cm.ExecuteNonQuery();
                }
                catch (SqlException sex)
                {
                    Env.WriteSqlToLog(sex, cm);
                    throw sex;
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex);
                    throw ex;
                }
                finally
                {
                    cm.Dispose();
                }
            }
        }

        public virtual void DB_Delete()
        {
            using (var cn = new SqlConnection(connectionString))
            {
                SqlTransaction tran = null;

                try
                {
                    cn.Open();
                    tran = cn.BeginTransaction(IsolationLevel.ReadCommitted);

                    DB_Delete(tran);

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex);
                    if (tran != null) tran.Rollback();
                    throw new Exception("Возникла ошибка при удалении.\n" + ex.Message);
                }
                finally
                {
                    cn.Close();
                }
            }
        }

        public virtual void DB_Delete(SqlTransaction tran)
        {
            using (var cm = new SqlCommand())
            {
                cm.Connection = tran.Connection;
                cm.Transaction = tran;

                cm.CommandText = string.Format("DELETE {0} WHERE {1}={2}",
                                               new object[] {FillFrom_Table, ID_Field, ID});

                try
                {
                    cm.ExecuteNonQuery();
                }
                catch (SqlException sex)
                {
                    Env.WriteSqlToLog(sex, cm);
                    throw sex;
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex);
                    throw ex;
                }
                finally
                {
                    cm.Dispose();
                }
            }
        }

        protected virtual void SaveToSqlParameters(Base original, SqlParameterCollection parameters)
        {
            parameters.Add(new SqlParameter
                        {
                            ParameterName = "@" + ID_Field,
                            Value = new SqlInt32(ID),
                            Direction = ParameterDirection.InputOutput
                        });
        }

        #endregion
    }
}
