using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading;

namespace Kesco.Lib.Win.Data.Business.V2.Docs.DomainObjects
{
    public class Field : Entity, IFormattable
    {
        private static readonly StringDictionary defaultMapping;

        public static StringDictionary DefaultMapping
        {
            get { return defaultMapping; }
        }

        public string _Name
        {
            get { return GetProp("name"); }
        }

        public string _NameEn
        {
            get { return GetProp("nameEn"); }
        }

        public string _ColumnName
        {
            get { return GetProp("columnName"); }
        }

        public string _FieldType
        {
            get { return GetProp("fieldType"); }
        }

        public string _DecimalScale
        {
            get { return GetProp("decimalScale"); }
        }

        public string _SearchUrl
        {
            get { return GetProp("searchUrl"); }
        }

        public string _SearchParams
        {
            get { return GetProp("searchParams"); }
        }

        public string _Caption
        {
            get { return GetProp("caption"); }
        }

        public string _IsRequired
        {
            get { return GetProp("isRequired"); }
        }

        public string _ConnectionString
        {
            get { return GetProp("connectionString"); }
        }

        public string _SqlQuery
        {
            get { return GetProp("sqlQuery"); }
        }

        public string _Description
        {
            get { return GetProp("dscr"); }
        }

        public string _Key
        {
            get { return GetProp("key"); }
        }

        public string Name
        {
            get { return _Name; }
        }

        public string ColumnName
        {
            get { return _ColumnName; }
        }

        public string Caption
        {
            get { return _Caption; }
        }

        public string Search_Params
        {
            get { return _SearchParams; }
        }

        public string Key
        {
            get { return _Key; }
        }

        public bool IsRequired
        {
            get { return Str2Bool(_IsRequired, false); }
        }

        public string TextLength
        {
            get
            {
                Match m = Regex.Match(_ColumnName, @"Text(\d*)_", RegexOptions.IgnoreCase);
                if (m.Success)
                    return m.Groups[1].Value;
                return "300";
            }
        }

        public int DecimalScale
        {
            get { return int.Parse(_DecimalScale); }
        }

        public string Search_SearchFormUrl
        {
            get { return TranslateURL(_SearchUrl) + "&title=" + Caption; }
        }

        public void FillItems(NameValueCollection items)
        {
            string cn = _ConnectionString;
            string query = _SqlQuery;

            if (string.IsNullOrEmpty(cn))
            {
                string[] arr = query.Split(';');
                int i = 0;
                while (i + 1 < arr.Length)
                    items.Add(arr[i++], arr[i++]);
                return;
            }
            cn = TranslateURL(cn);

            using (var dt = new DataTable())
            using (var da = new SqlDataAdapter(query, cn))
            {
                try
                {
                    da.Fill(dt);
                    da.SelectCommand.Connection.Close();
                    for (int i = 0; i < dt.Rows.Count; i++)
                        items.Add(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString());
                }
                catch (SqlException sex)
                {
                    Env.WriteSqlToLog(sex, da.SelectCommand);
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex);
                }
                finally
                {
                    da.SelectCommand.Connection.Close();
                    da.Dispose();
                }
            }
        }

        public string ListItems
        {
            get
            {
                string cn = _ConnectionString;
                string query = _SqlQuery;

                if (string.IsNullOrEmpty(cn))
                    return query;
                cn = TranslateURL(cn);
                string list = "";

                using (var da = new SqlDataAdapter(query, cn))
                    try
                    {
                        da.SelectCommand.Connection.Open();
                        using (SqlDataReader reader = da.SelectCommand.ExecuteReader())
                        {
                            while (reader.Read())
                                list += (list.Length > 0 ? ";" : "") + reader[0] + ";" + reader[1];

                            reader.Close();
                        }
                    }
                    catch (SqlException sex)
                    {
                        Env.WriteSqlToLog(sex, da.SelectCommand);
                    }
                    catch (Exception ex)
                    {
                        Env.WriteToLog(ex);
                    }
                    finally
                    {
                        da.SelectCommand.Connection.Close();
                    }

                return list;
            }
        }

        public override void Load()
        {
            var dt = new DataTable();
            Env.Docs.Find("SELECT * FROM ПоляДокументов WHERE КодПоляДокумента=" + _ID, dt);
            if (dt.Rows.Count == 1) Populate(dt.Rows[0], DefaultMapping);
            else props["unavailable"] = "1";
        }

        static Field()
        {
            defaultMapping = new StringDictionary
                                 {
                                     {"КодПоляДокумента", "id"},
                                     {"КодТипаДокумента", "docType"},
                                     {"ПорядокПоляДокумента", "order"},
                                     {"ПолеДокумента", "name"},
                                     {"ПолеДокументаEn", "nameEn"},
                                     {"КолонкаТаблицы", "columnName"},
                                     {"Обязательность", "isRequired"},
                                     {"КодТипаПоля", "fieldType"},
                                     {"ЧислоДесятичныхЗнаков", "decimalScale"},
                                     {"URLПоиска", "searchUrl"},
                                     {"ПараметрыПоиска", "searchParams"},
                                     {"ЗаголовокФормыПоиска", "caption"},
                                     {"Идентификатор", "key"},
                                     {"СтрокаПодключения", "connectionString"},
                                     {"Описание", "dscr"},
                                     {"SQLЗапрос", "sqlQuery"},
                                     {"Изменил", "editor"},
                                     {"Изменено", "edited"}
                                 };
        }

        public Field(string id)
            : base(id, "DocFld", Env.Docs)
        {
        }

        public static string TranslateURL(string url)
        {
            return Regex.Replace(url, "@([_A-ZА-Я0-9]+)", ReplaceCC, RegexOptions.IgnoreCase);
        }

        private static string ReplaceCC(Match m)
        {
            return ConfigurationSettings.AppSettings[m.Groups[1].Value];
        }

        #region IFormattable Members

        public string ToString(string format, IFormatProvider formatProvider)
        {
            string lang = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;

            switch (lang)
            {
                case "ru":
                    return _Name;
                default:
                    return _NameEn;
            }
        }

        #endregion
    }
}