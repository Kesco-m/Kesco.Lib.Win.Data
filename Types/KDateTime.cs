using System;
using System.Data;
using System.Data.SqlClient;
using Sql=System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.Types
{
    /// <summary>
    /// Summary description for KDateTime.
    /// </summary>
    public class KDateTime : KType
    {
        private DateTime dt;

        //DB Save 
        //имя параметра ("")	SqlParameterName
        //тип параметра	(SqlDbType.DateTime) SqlParameterType

        //Load
        //имя колонки	SqlColumnName

        //XML attributeName
        //XmlAttributeName

        //public static string UtcToXml(DateTime dt, string format)
        //{
        //    return System.Xml.XmlConvert.ToString(dt).Substring(0,19)+format;
        //}

        public override string Xml
        {
            get { return isUndefined ? "" : dt.ToString("dd.MM.yyyy"); }
            set
            {
                isUndefined = true;
                if (string.IsNullOrEmpty(value))
                    return;
                dt = DateTime.Parse(value);

                isUndefined = false;
            }
        }

        #region DB

        public override void Fill(DataRow row)
        {
            if (isUndefined = row.IsNull(SqlColumnName))
                return;
            dt = (DateTime) row[SqlColumnName];
        }

        public object SqlParameter
        {
            get
            {
                return new SqlParameter
                            {
                                SqlDbType = SqlDbType.DateTime,
                                ParameterName = "",
                                Value = IsUndefined ? DBNull.Value : (object) dt
                            };
            }
        }

        #endregion

        public string Text
        {
            get { return string.Empty; }
            set { dt = DateTime.Parse(value); }
        }

        public KDateTime()
            : base("", "", SqlDbType.Int, "")
        {
        }
    }
}