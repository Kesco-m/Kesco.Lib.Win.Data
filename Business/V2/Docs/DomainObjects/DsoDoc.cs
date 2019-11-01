using System;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Kesco.Lib.Win.Data.Business.V2.FilterOptions;
using Kesco.Lib.Win.Data.Temp;

namespace Kesco.Lib.Win.Data.Business.V2.Docs.DomainObjects
{
    public class DsoDoc : Dso
    {
        #region Классы опций

        public class IDsOption : FOptInt
        {
            //допустимо объединение только по или
            private const string id_xml = "КодДокумента";

            protected override void RenderSqlLValue()
            {
                ds.w.Write("T0.КодДокумента");
            }

            internal override void GetIDs(string[] ids, ref int i)
            {
                base.GetIDs(ids, ref i);
                ids[i++] = id_xml;
            }

            public IDsOption(Dso filter, string id)
                : base(filter, id)
            {
                description = "код документа";

                flags = FOptFlags.MatchAnyItem;
                flagsMask = FOptFlags.Enabled | FOptFlags.Fixed | FOptFlags.Inverse;
                itemFlagsDefault = FOptItemFlags.Equals;
                itemFlagsMask = FOptItemFlags.IsNull | FOptItemFlags.Equals | FOptItemFlags.Less | FOptItemFlags.More;

            }
        }

        public class SearchTextOption : FOptText
        {
            private const string id_xml = "СтрокаПоиска";

            private string sql = "";

            public override bool RenderSqlPrepClause()
            {
                if (!base.RenderSqlPrepClause()) return false;

                string val = Text;
                val = val.Replace("№", "№ ").Trim();
                string[] words = Regex.Split(val, @"\s+");
                string typeids = "", numberwords = "", date = "", id = "";
                string word;
                for (int i = 0; i < words.Length; i++)
                {
                    word = words[i];

                    if (Regex.IsMatch(word, "от|ot|oт|оt", RegexOptions.IgnoreCase))
                        continue;
                    if (Regex.IsMatch(word, "№", RegexOptions.IgnoreCase))
                        continue;
                    if (IsID(words, i, ref id))
                        continue;
                    if (IsType(word, ref typeids))
                        continue;

                    if (i > 0 && words[i - 1].Equals("№"))
                    {
                        if (IsNumber(word, ref numberwords))
                            continue;
                    }
                    else
                    {
                        if (IsDate(word, ref date))
                            continue;
                        if (IsNumber(word, ref numberwords))
                            continue;
                    }
                }

                if (id.Length > 0 && numberwords.Length == 0)
                    numberwords = id;

                if (date.Length > 0)
                    sql += (sql.Length == 0 ? "" : " AND ") + "(T0.ДатаДокумента = '" + date + "')";
                if (numberwords.Length > 0)
                {
                    numberwords = SqlEscape(GetWords(numberwords, "[^ ]+")).Replace(" ", "%") + "%";

                    sql += (sql.Length == 0 ? "" : " AND ") + "(T0.НомерДокументаRL LIKE '" +
                           Replacer.ReplaceRusLat(numberwords) + "')";
                }
                if (typeids.Length > 0)
                    sql += (sql.Length == 0 ? "" : " AND ") + "(T0.КодТипаДокумента IN (" + typeids + "))";


                if (id.Length > 0)
                    sql = (sql.Length == 0 ? "" : "(" + sql + ") OR ") + "(T0.КодДокумента=" + id + ")";

                return sql.Length > 0;
            }

            public override void RenderSqlWhereClause()
            {
                ds.w.Write("(");
                ds.w.Write(sql);
                ds.w.Write(")");
            }

            private static bool IsID(string[] words, int index, ref string id)
            {
                if (index > 0 || words.Length > 1)
                    return false;
                if (!Regex.IsMatch(words[index], "^\\d+$"))
                    return false;

                var dt = new DataTable();
                Env.Docs.Find(
                    "SELECT КодДокумента FROM Документы.dbo.vwДокументы (nolock) WHERE КодДокумента=" + words[index], dt);
                if (dt.Rows.Count != 1)
                    return false;

                id = words[index];
                return true;
            }

            private static bool IsDate(string str, ref string date)
            {
                if (!Regex.IsMatch(str, "^[0-9]{1,4}[.,/-][0-9]{1,4}[.,/-][0-9]{1,4}$"))
                    return false;
                try
                {
                    date = NewDateParser.Parse(str).ToString("yyyyMMdd");
                    return true;
                }
                catch (Exception ex)
                {
                    Env.WriteToLog(ex, str);
                }
                return false;
            }

            private static bool IsType(string str, ref string typeids)
            {
                string sql =
                    @"
SELECT КодТипаДокумента FROM Документы.dbo.ТипыДокументов
WHERE ' '+ТипДокумента like '% " +
                    SqlEscape(GetWords(str, "[0-9A-ZА-Я_]+")).Replace(" ", "%") + @"%'
";
                if (typeids.Length > 0)
                    sql += " AND КодТипаДокумента IN(" + typeids + ")";

                using (var dt = new DataTable())
                {
                    Env.Docs.Find(sql, dt);

                    if (dt.Rows.Count == 0)
                        return false;
                    typeids = "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                        typeids += (typeids.Length == 0 ? "" : ",") + dt.Rows[i][0];
                }

                return true;
            }

            private static bool IsNumber(string str, ref string numberwords)
            {
                if (string.IsNullOrEmpty(str))
                    return false;

                numberwords += str + " ";
                return true;
            }

            internal override void GetIDs(string[] ids, ref int i)
            {
                base.GetIDs(ids, ref i);
                ids[i++] = id_xml;
            }

            public SearchTextOption(Dso filter, string id) : base(filter, id)
            {
                description = "документ";
                flagsMask = FOptFlags.Enabled | FOptFlags.Fixed;
            }
        }

        public class PersonOption : FOptPerson
        {
            private const string id_xml = "ЛицаКонтрагенты";
            private const string id_url = "persons";

            public override void RenderSqlWhereClause()
            {
                if (!MatchAnyItem || items.Count == 1)
                {
                    base.RenderSqlWhereClause();
                    return;
                }

                if (Inverse) ds.w.Write("NOT ");
                ds.w.Write("EXISTS (SELECT * FROM Документы.dbo.vwЛицаДокументов TI (nolock) ");
                ds.w.Write("WHERE TI.КодДокумента=T0.КодДокумента AND TI.КодЛица IN (");
                RenderCSVList();
                ds.w.Write("))");
            }

            protected override void RenderSqlWhereClauseItem()
            {
                ds.w.Write("EXISTS (SELECT * FROM Документы.dbo.vwЛицаДокументов TI (nolock) ");
                ds.w.Write("WHERE TI.КодДокумента=T0.КодДокумента AND TI.КодЛица = {0})", curItem.value);
            }

            internal override void GetIDs(string[] ids, ref int i)
            {
                base.GetIDs(ids, ref i);
                ids[i++] = id_xml;
                ids[i++] = id_url;
            }

            public PersonOption(Dso filter, string id) : base(filter, id)
            {
                description = "лица-контрагенты";

                flags = FOptFlags.None;
                flagsMask = FOptFlags.Enabled | FOptFlags.Fixed | FOptFlags.Inverse | FOptFlags.MatchAnyItem;
                itemFlagsDefault = FOptItemFlags.Equals;
                itemFlagsMask = FOptItemFlags.None;
            }
        }

        public class TypeOption : FOptInt
        {
            private const string id_xml = "ТипДокумента";
            private const string id_url = "types";

            public override bool RenderSqlPrepClause()
            {
                if (!base.RenderSqlPrepClause())
                    return false;

                ds.w.Write(
                    @"
                        DECLARE @Types TABLE(КодТипаДокумента int, L int, R int, Ch int)
                        DECLARE @ParentType int
                        ");

                //int ch;
                foreach (FOptItem item in items)
                {
                    //ch = ((item.flags & FOptItemFlags.ChildOf) == FOptItemFlags.ChildOf) ? 1 : 0;
                    if ((item.flags & FOptItemFlags.SameAs) == FOptItemFlags.SameAs)
                        ds.w.Write(
                            @"
                            SET @ParentType = (SELECT CASE WHEN ТипДокумента IS Null THEN КодТипаДокумента ELSE Parent END FROM ТипыДокументов WHERE КодТипаДокумента={0})
                            INSERT @Types SELECT  КодТипаДокумента, L, R, 1 FROM ТипыДокументов WHERE (@ParentType IS NULL AND КодТипаДокумента={0}) OR (@ParentType IS NOT NULL AND Parent = @ParentType)
                            ",
                            item.value);
                    else
                        ds.w.Write(
                            "INSERT @Types SELECT КодТипаДокумента, L, R, 1  FROM ТипыДокументов WHERE КодТипаДокумента={0}",
                            item.value);
                }

                ds.w.Write(
                    @"
                    INSERT @Types SELECT T0.КодТипаДокумента, T0.L, T0.R, 0  FROM ТипыДокументов T0
                    INNER JOIN @Types AS T1 ON T1.Ch=1 AND T1.L<T0.L AND T0.R<T1.R


                    ");
                return true;
            }

            public override void RenderSqlWhereClause()
            {
                if (Inverse)
                    ds.w.Write("NOT ");
                ds.w.Write("EXISTS (SELECT * FROM @Types TI WHERE T0.КодТипаДокумента=TI.КодТипаДокумента)");
            }

            internal override void GetIDs(string[] ids, ref int i)
            {
                base.GetIDs(ids, ref i);
                ids[i++] = id_xml;
                ids[i++] = id_url;
            }

            public TypeOption(Dso filter, string id)
                : base(filter, id)
            {
                description = "тип документа";

                flags = FOptFlags.MatchAnyItem;
                flagsMask = FOptFlags.Enabled | FOptFlags.Fixed | FOptFlags.Inverse;
                itemFlagsDefault = FOptItemFlags.Equals | FOptItemFlags.ChildOf | FOptItemFlags.SameAs;
                itemFlagsMask = FOptItemFlags.ChildOf | FOptItemFlags.SameAs;
            }
        }

        public class NumberOption : FOptText
        {
            private const string id_xml = "Document.Номер";

            protected override void RenderSqlLValue()
            {
                ds.w.Write(" T0.НомерДокумента");
            }

            internal override void GetIDs(string[] ids, ref int i)
            {
                base.GetIDs(ids, ref i);
                ids[i++] = id_xml;
            }

            public NumberOption(Dso filter, string id) : base(filter, id)
            {
                description = "номер документа";
                flags = FOptFlags.TextEquals;
                flagsMask = FOptFlags.TextEquals | FOptFlags.TextBeginsWith | FOptFlags.Enabled | FOptFlags.Fixed |
                            FOptFlags.Inverse;
                itemFlagsDefault = FOptItemFlags.Equals;
                itemFlagsMask = FOptItemFlags.None;
            }
        }

        public class DateOption : FOptDateTime
        {
            private const string id_xml = "Дата";

            protected override void RenderSqlLValue()
            {
                ds.w.Write("T0.ДатаДокумента");
            }

            internal override void GetIDs(string[] ids, ref int i)
            {
                base.GetIDs(ids, ref i);
                ids[i++] = id_xml;
            }

            public DateOption(Dso filter, string id) : base(filter, id)
            {
                description = "дата документа";
            }
        }

        public class DescriptionOption : FOptText
        {
            private const string id_xml = "Описание";

            internal override void GetIDs(string[] ids, ref int i)
            {
                base.GetIDs(ids, ref i);
                ids[i++] = id_xml;
            }

            public DescriptionOption(Dso filter, string id)
                : base(filter, id)
            {
                description = "описание";
            }
        }

        public class LinkedDocOption : FOptDoc
        {
            private const string id_xml = "СвязанСДокументом";
            public string _Field { get; set; } //коллекция
            //Parents означает что у выбранного документа существуют потомки ксуществуют документы-основания

            protected override void RenderSqlWhereClauseItem()
            {
                StringCollection col = Entity.Str2Collection(_Field);
                for (int i = col.Count - 1; i >= 0; i--) if (!Regex.IsMatch(col[i], "^\\d+$")) col.RemoveAt(i);

                bool c = (curItem.flags & FOptItemFlags.ChildOf) == FOptItemFlags.ChildOf;
                bool c0 = (curItem.flags & FOptItemFlags.DirectChildOf) == FOptItemFlags.DirectChildOf;
                c0 &= !c;
                bool p = (curItem.flags & FOptItemFlags.ParentOf) == FOptItemFlags.ParentOf;
                bool p0 = (curItem.flags & FOptItemFlags.DirectParentOf) == FOptItemFlags.DirectParentOf;
                p0 &= !p;

                if ((c || c0) && (p || p0)) ds.w.Write("(");

                //искомый документ является основанием документа {0}
                if (p)
                    ds.w.Write(
                        "EXISTS (SELECT * FROM Документы.dbo.fn_ВсеОснования({0}) TI Where TI.КодДокумента=T0.КодДокумента)",
                        curItem.value);
                //искомый документ является непосредственным основанием документа {0}
                if (p0)
                {
                    ds.w.Write(
                        "EXISTS (SELECT * FROM Документы.dbo.vwСвязиДокументов TI (nolock) WHERE TI.КодДокументаВытекающего={0} AND TI.КодДокументаОснования=T0.КодДокумента",
                        curItem.value);
                    if (col.Count > 0) ds.w.Write(" AND TI.КодПоляДокумента IN ({0})", Entity.Collection2Str(col));
                    ds.w.Write(")");
                }

                if ((c || c0) && (p || p0)) ds.w.Write(" OR ");

                //искомый документ вытекает из документа {0}
                if (c)
                    ds.w.Write(
                        "EXISTS (SELECT * FROM Документы.dbo.fn_ВсеВытекающие({0}) TI Where TI.КодДокумента=T0.КодДокумента)",
                        curItem.value);
                //искомый документ непосредственно вытекает из документа {0}
                if (c0)
                {
                    ds.w.Write(
                        "EXISTS (SELECT * FROM Документы.dbo.vwСвязиДокументов TI (nolock) WHERE TI.КодДокументаОснования={0} AND TI.КодДокументаВытекающего=T0.КодДокумента",
                        curItem.value);
                    if (col.Count > 0) ds.w.Write(" AND TI.КодПоляДокумента IN ({0})", Entity.Collection2Str(col));
                    ds.w.Write(")");
                }
                if ((c || c0) && (p || p0)) ds.w.Write(")");
            }

            internal override void GetIDs(string[] ids, ref int i)
            {
                base.GetIDs(ids, ref i);
                ids[i++] = id_xml;
            }

            public LinkedDocOption(Dso filter, string id) : base(filter, id)
            {
                _Field = "";

                description = "документ связан с другим документом";

                flags = FOptFlags.None;
                flagsMask = FOptFlags.Enabled | FOptFlags.Fixed | FOptFlags.Inverse | FOptFlags.MatchAnyItem;
                itemFlagsDefault = FOptItemFlags.DirectChildOf | FOptItemFlags.DirectParentOf;
                itemFlagsMask = FOptItemFlags.ChildOf | FOptItemFlags.DirectChildOf | FOptItemFlags.ParentOf |
                                FOptItemFlags.DirectParentOf;
            }
        }

        public class EFormOption : FOpt
        {
            #region КлассыОпций

            public class ExistsOption : FOpt
            {
                private const string id_xml = "ЭлФорма";

                public override void RenderSqlWhereClause()
                {
                    if (Inverse) ds.w.Write("NOT ");
                    ds.w.Write(
                        "EXISTS (SELECT * FROM vwДокументыДанные TI (nolock) WHERE TI.КодДокумента=T0.КодДокумента)");
                }

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public ExistsOption(Dso filter, string id) : base(filter, id)
                {
                    description = "документ имеет электронную форму";
                    flags = FOptFlags.None;
                    flagsMask = FOptFlags.Enabled | FOptFlags.Fixed | FOptFlags.Inverse;
                    itemFlagsDefault = FOptItemFlags.None;
                    itemFlagsMask = FOptItemFlags.None;
                }
            }

            public class NotExistsOption : FOpt
            {
                private const string id_xml = "ЭлФормаОтсутствует";

                public override void RenderSqlWhereClause()
                {
                    if (!Inverse) ds.w.Write("NOT ");
                    ds.w.Write(
                        "EXISTS (SELECT * FROM vwДокументыДанные TI (nolock) WHERE TI.КодДокумента=T0.КодДокумента)");
                }

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public NotExistsOption(Dso filter, string id) : base(filter, id)
                {
                    description = "документ не имеет электронной формы";

                    flags = FOptFlags.None;
                    flagsMask = FOptFlags.Enabled | FOptFlags.Fixed | FOptFlags.Inverse;
                    itemFlagsDefault = FOptItemFlags.None;
                    itemFlagsMask = FOptItemFlags.None;
                }
            }

            public class ChangedByOption : FOptEmployee
            {
                private const string id_xml = "EForm.ChangedBy";

                public override void RenderSqlWhereClause()
                {

                    if (Inverse) ds.w.Write("NOT ");
                    ds.w.Write(
                        "EXISTS (SELECT * FROM vwДокументыДанные TI (nolock) WHERE TI.КодДокумента=T0.КодДокумента AND ");

                    if (items.Count > 1)
                    {
                        ds.w.Write("TI.Изменил IN (");
                        RenderCSVList();
                        ds.w.Write(")");
                    }
                    if (items.Count == 1) ds.w.Write("TI.Изменил={0}", ((FOptItem) items[0]).value);

                    ds.w.Write(")");
                }

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public ChangedByOption(Dso filter, string id) : base(filter, id)
                {
                    description = "эл.форма изменена сотрудником";

                    flags = FOptFlags.MatchAnyItem;
                    flagsMask = FOptFlags.Enabled | FOptFlags.Fixed | FOptFlags.Inverse;
                    itemFlagsDefault = FOptItemFlags.Equals;
                    itemFlagsMask = FOptItemFlags.None;
                }
            }

            #endregion

            private ExistsOption exists;
            private NotExistsOption notExists;
            private ChangedByOption changedBy;

            #region ACCESSORS

            public ExistsOption Exists
            {
                get { return exists; }
            }

            public NotExistsOption NotExists
            {
                get { return notExists; }
            }

            public ChangedByOption ChangedBy
            {
                get { return changedBy; }
            }

            #endregion

            public EFormOption(Dso filter, string id) : base(filter, id)
            {
                exists = new ExistsOption(filter, id + ".exists");
                notExists = new NotExistsOption(filter, id + ".notExists");
                changedBy = new ChangedByOption(filter, id + ".changedBy");

                is4Sql = false;
                is4User = false;
            }
        }

        public class ImageOption : FOpt
        {
            #region КлассыОпций

            public class ExistsOption : FOpt
            {
                private const string id_xml = "Изображение";

                public override void RenderSqlWhereClause()
                {
                    if (Inverse) ds.w.Write("NOT ");
                    ds.w.Write("EXISTS (SELECT * FROM vwИзображенияДокументов TI WHERE TI.КодДокумента=T0.КодДокумента)");
                }

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public ExistsOption(Dso filter, string id) : base(filter, id)
                {
                    description = "документ имеет изображение";

                    flags = FOptFlags.None;
                    flagsMask = FOptFlags.Enabled | FOptFlags.Fixed | FOptFlags.Inverse;
                    itemFlagsDefault = FOptItemFlags.None;
                    itemFlagsMask = FOptItemFlags.None;
                }
            }

            public class NotExistsOption : FOpt
            {
                private const string id_xml = "ИзображениеОтсутствует";

                public override void RenderSqlWhereClause()
                {
                    if (!Inverse) ds.w.Write("NOT ");
                    ds.w.Write("EXISTS (SELECT * FROM vwИзображенияДокументов TI WHERE TI.КодДокумента=T0.КодДокумента)");
                }

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public NotExistsOption(Dso filter, string id) : base(filter, id)
                {
                    description = "документ не имеет изображения";
                    flags = FOptFlags.None;
                    flagsMask = FOptFlags.Enabled | FOptFlags.Fixed | FOptFlags.Inverse;
                    itemFlagsDefault = FOptItemFlags.None;
                    itemFlagsMask = FOptItemFlags.None;
                }
            }

            public class ScanedOption : FOptDateTime
            {
                private const string id_xml = "ДатаАрхивирования";

                public override void RenderSqlWhereClause()
                {
                    ds.w.Write(
                        "EXISTS (SELECT * FROM vwИзображенияДокументов TI WHERE TI.КодДокумента=T0.КодДокумента AND ");
                    base.RenderSqlWhereClause();
                    ds.w.Write(")");
                }

                protected override void RenderSqlLValue()
                {
                    ds.w.Write("TI.Сохранено");
                }

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public ScanedOption(Dso filter, string id) : base(filter, id)
                {
                    description = "дата поступления изображения документа";
                }
            }

            public class ArchiveOption : FOptInt
            {
                private const string id_xml = "Хранилище";

                public override bool RenderSqlPrepClause()
                {
                    if (!base.RenderSqlPrepClause()) return false;

                    ds.w.Write(
                        @"
DECLARE @ImageArchives TABLE(КодХранилища int)
DECLARE @ImageArchiveL int
DECLARE @ImageArchiveR int
");

                    foreach (FOptItem item in items)
                    {
                        if ((item.flags & FOptItemFlags.ChildOf) == FOptItemFlags.ChildOf)
                            ds.w.Write(
                                @"
SELECT @ImageArchiveL=L,@ImageArchiveR=R FROM Хранилища WHERE КодХранилища={0}
INSERT @ImageArchives SELECT КодХранилища FROM Хранилища WHERE @ImageArchiveL<=L AND R<=@ImageArchiveR",
                                item.value);
                        else
                            ds.w.Write("INSERT @ImageArchives VALUES ({0})", item.value);
                    }

                    ds.w.Write("\r\n\r\n");
                    return true;
                }

                public override void RenderSqlWhereClause()
                {
                    if (Inverse) ds.w.Write("NOT ");
                    ds.w.Write(
                        @"EXISTS (SELECT * FROM vwИзображенияДокументов TI 
INNER JOIN @ImageArchives T2 ON TI.КодХранилища = T2.КодХранилища
WHERE TI.КодДокумента=T0.КодДокумента)");
                }

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public ArchiveOption(Dso filter, string id) : base(filter, id)
                {
                    description = "оригинал документа хранится в хранилище";

                    flags = FOptFlags.MatchAnyItem;
                    flagsMask = FOptFlags.Enabled | FOptFlags.Fixed | FOptFlags.Inverse | FOptFlags.MatchAnyItem;
                    itemFlagsDefault = FOptItemFlags.Equals;
                    itemFlagsMask = FOptItemFlags.ChildOf;
                }
            }


            #endregion

            private ExistsOption exists;
            private NotExistsOption notExists;
            private ScanedOption scaned;
            private ArchiveOption archive;

            #region ACCESSORS

            public ScanedOption Scaned
            {
                get { return scaned; }
            }

            public ArchiveOption Archive
            {
                get { return archive; }
            }

            public ExistsOption Exists
            {
                get { return exists; }
            }

            public NotExistsOption NotExists
            {
                get { return notExists; }
            }

            #endregion

            public ImageOption(Dso filter, string id) : base(filter, id)
            {
                exists = new ExistsOption(filter, id + ".exists");
                notExists = new NotExistsOption(filter, id + ".notExists");
                scaned = new ScanedOption(filter, id + ".scaned");
                archive = new ArchiveOption(filter, id + ".archive");

                is4Sql = false;
                is4User = false;
            }
        }

        public class SignOption : FOpt
        {
            #region КлассыОпций

            public class FinishedOption : FOpt
            {
                private const string id_xml = "Выполнен";

                public override void RenderSqlWhereClause()
                {
                    if (Inverse) ds.w.Write("NOT ");
                    ds.w.Write(
                        "EXISTS (SELECT * FROM ПодписиДокументов TI WHERE TI.КодДокумента=T0.КодДокумента AND TI.ТипПодписи=1)");
                }

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public FinishedOption(Dso filter, string id) : base(filter, id)
                {
                    description = "документ выполнен";

                    flags = FOptFlags.None;
                    flagsMask = FOptFlags.Enabled | FOptFlags.Fixed | FOptFlags.Inverse;
                    itemFlagsDefault = FOptItemFlags.None;
                    itemFlagsMask = FOptItemFlags.None;
                }
            }

            public class NotFinishedOption : FOpt
            {
                private const string id_xml = "НеВыполнен";

                public override void RenderSqlWhereClause()
                {
                    if (!Inverse) ds.w.Write("NOT ");
                    ds.w.Write(
                        "EXISTS (SELECT * FROM ПодписиДокументов TI WHERE TI.КодДокумента=T0.КодДокумента AND TI.ТипПодписи=1)");
                }

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public NotFinishedOption(Dso filter, string id) : base(filter, id)
                {
                    description = "документ не выполнен";

                    flags = FOptFlags.None;
                    flagsMask = FOptFlags.Enabled | FOptFlags.Fixed | FOptFlags.Inverse;
                    itemFlagsDefault = FOptItemFlags.None;
                    itemFlagsMask = FOptItemFlags.None;
                }
            }

            public class SignedByOption : FOptEmployee
            {
                private const string id_xml = "Подписан";

                public override bool RenderSqlPrepClause()
                {
                    return Enabled;
                }

                public override void RenderSqlWhereClause()
                {

                    if (Inverse) ds.w.Write("NOT ");
                    ds.w.Write("EXISTS (SELECT * FROM ПодписиДокументов TI WHERE TI.КодДокумента=T0.КодДокумента");

                    if (items.Count > 0)
                    {
                        ds.w.Write(" AND (");
                        base.RenderSqlWhereClause();
                        ds.w.Write(")");

                    }

                    ds.w.Write(")");
                }

                protected override void RenderSqlWhereClauseItem()
                {
                    ds.w.Write("TI.КодСотрудникаЗа={0} OR TI.КодСотрудника={0}", curItem.value);
                }

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public SignedByOption(Dso filter, string id) : base(filter, id)
                {
                    description = "документ подписан";

                    flags = FOptFlags.MatchAnyItem;
                    flagsMask = FOptFlags.Enabled | FOptFlags.Fixed | FOptFlags.Inverse | FOptFlags.MatchAnyItem;
                    itemFlagsDefault = FOptItemFlags.Equals;
                    itemFlagsMask = FOptItemFlags.None;
                }
            }

            public class NotSignedByOption : FOptEmployee
            {
                private const string id_xml = "НеПодписан";

                public override bool RenderSqlPrepClause()
                {
                    return Enabled;
                }

                public override void RenderSqlWhereClause()
                {

                    if (!Inverse) ds.w.Write("NOT ");
                    ds.w.Write("EXISTS (SELECT * FROM ПодписиДокументов TI WHERE TI.КодДокумента=T0.КодДокумента");
                    if (items.Count > 0)
                    {
                        ds.w.Write(" AND (");
                        base.RenderSqlWhereClause();
                        ds.w.Write(")");
                    }
                    ds.w.Write(")");
                }

                protected override void RenderSqlWhereClauseItem()
                {
                    ds.w.Write("TI.КодСотрудникаЗа={0} OR TI.КодСотрудника={0}", curItem.value);
                }

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public NotSignedByOption(Dso filter, string id)
                    : base(filter, id)
                {
                    description = "документ не подписан";

                    flags = FOptFlags.MatchAnyItem;
                    flagsMask = FOptFlags.Enabled | FOptFlags.Fixed | FOptFlags.Inverse | FOptFlags.MatchAnyItem;
                    itemFlagsDefault = FOptItemFlags.Equals;
                    itemFlagsMask = FOptItemFlags.None;
                }
            }

            #endregion

            private FinishedOption finished;
            private NotFinishedOption notFinished;
            private SignedByOption signedBy;
            private NotSignedByOption notSignedBy;

            #region ACCESSORS

            public FinishedOption Finished
            {
                get { return finished; }
            }

            public NotFinishedOption NotFinished
            {
                get { return notFinished; }
            }

            public SignedByOption SignedBy
            {
                get { return signedBy; }
            }

            public NotSignedByOption NotSignedBy
            {
                get { return notSignedBy; }
            }

            #endregion

            public SignOption(Dso filter, string id) : base(filter, id)
            {
                finished = new FinishedOption(filter, id + ".finished");
                notFinished = new NotFinishedOption(filter, id + ".notFinished");
                signedBy = new SignedByOption(filter, id + ".signedBy");
                notSignedBy = new NotSignedByOption(filter, id + ".notSignedBy");
                is4Sql = false;
                is4User = false;
            }
        }

        public class MsgInOption : FOpt
        {
            #region КлассыОпций

            public class TextOption : FOptText
            {
                private const string id_xml = "Message.Incoming.Text";

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public TextOption(Dso filter, string id) : base(filter, id)
                {
                    is4Sql = false;
                    description = "текст полученного сообщения";
                }
            }

            public class ByOption : FOptEmployee
            {
                private const string id_xml = "Message.Incoming.By";

                public override bool RenderSqlPrepClause()
                {
                    return Enabled;
                }

                public override void RenderSqlWhereClause()
                {
                    ds.w.Write("TI.КодСотрудникаОтправителя");
                    if (Inverse) ds.w.Write(" NOT");
                    ds.w.Write(" IN (");
                    RenderCSVList();
                    ds.w.Write(")");
                }

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public ByOption(Dso filter, string id) : base(filter, id)
                {
                    description = "сообщение получено от сотрудника";
                    is4Sql = false;

                    flags = FOptFlags.MatchAnyItem;
                    flagsMask = FOptFlags.Enabled | FOptFlags.Fixed | FOptFlags.Inverse;
                    itemFlagsDefault = FOptItemFlags.Equals;
                    itemFlagsMask = FOptItemFlags.None;
                }
            }

            public class DateOption : FOptDateTime
            {
                private const string id_xml = "Message.Incoming.Date";

                protected override void RenderSqlLValue()
                {
                    ds.w.Write("TI.Отправлено");
                }

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public DateOption(Dso filter, string id) : base(filter, id)
                {
                    is4Sql = false;
                    description = "дата получения сообщения";
                }
            }

            public class ReadOption : FOpt
            {
                private const string id_xml = "Message.Incoming.Read";

                public override void RenderSqlWhereClause()
                {
                    ds.w.Write("TI.Прочитано IS");
                    if (Inverse) ds.w.Write(" NOT");
                    ds.w.Write(" NULL");
                }

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public ReadOption(Dso filter, string id) : base(filter, id)
                {
                    is4Sql = false;
                    description = "полученное сообщение прочитано";

                    flags = FOptFlags.None;
                    flagsMask = FOptFlags.Enabled | FOptFlags.Fixed;
                    itemFlagsDefault = FOptItemFlags.None;
                    itemFlagsMask = FOptItemFlags.None;
                }
            }

            public class UnreadOption : FOpt
            {
                private const string id_xml = "Message.Incoming.Unread";

                public override void RenderSqlWhereClause()
                {
                    ds.w.Write("TI.Прочитано IS");
                    if (!Inverse) ds.w.Write(" NOT");
                    ds.w.Write(" NULL");
                }

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public UnreadOption(Dso filter, string id) : base(filter, id)
                {
                    is4Sql = false;
                    description = "полученное сообщение не прочитано";

                    flags = FOptFlags.None;
                    flagsMask = FOptFlags.Enabled | FOptFlags.Fixed;
                    itemFlagsDefault = FOptItemFlags.None;
                    itemFlagsMask = FOptItemFlags.None;
                }
            }

            #endregion

            private bool renderText;
            private bool renderBy;
            private bool renderDate;
            private bool renderRead;
            private bool renderUnread;

            private TextOption text;
            private ByOption by;
            private DateOption date;
            private ReadOption read;
            private UnreadOption unread;

            #region ACCESSORS

            public TextOption Text
            {
                get { return text; }
            }

            public ByOption By
            {
                get { return by; }
            }

            public DateOption Date
            {
                get { return date; }
            }

            public ReadOption Read
            {
                get { return read; }
            }

            public UnreadOption Unread
            {
                get { return unread; }
            }

            #endregion

            public override bool RenderSqlPrepClause()
            {
                renderText = text.RenderSqlPrepClause();
                renderBy = by.RenderSqlPrepClause();
                renderDate = date.RenderSqlPrepClause();
                renderRead = read.RenderSqlPrepClause();
                renderUnread = unread.RenderSqlPrepClause();
                return renderText || renderBy || renderDate || renderRead || renderUnread;
            }

            public override void RenderSqlWhereClause()
            {
                if (Inverse) ds.w.Write("NOT ");
                ds.w.Write("EXISTS (SELECT * FROM vwСообщенияВходящие TI WHERE TI.КодДокумента=T0.КодДокумента");

                if (renderText)
                {
                    ds.w.Write(" AND (");
                    text.RenderSqlWhereClause();
                    ds.w.Write(")");
                }
                if (renderBy)
                {
                    ds.w.Write(" AND (");
                    by.RenderSqlWhereClause();
                    ds.w.Write(")");
                }
                if (renderDate)
                {
                    ds.w.Write(" AND (");
                    date.RenderSqlWhereClause();
                    ds.w.Write(")");
                }
                if (renderRead)
                {
                    ds.w.Write(" AND (");
                    read.RenderSqlWhereClause();
                    ds.w.Write(")");
                }
                if (renderUnread)
                {
                    ds.w.Write(" AND (");
                    unread.RenderSqlWhereClause();
                    ds.w.Write(")");
                }

                ds.w.Write(")");
            }

            public MsgInOption(Dso filter, string id) : base(filter, id)
            {
                text = new TextOption(filter, id + ".text");
                by = new ByOption(filter, id + ".by");
                date = new DateOption(filter, id + ".date");
                read = new ReadOption(filter, id + ".read");
                unread = new UnreadOption(filter, id + ".unread");
                is4User = false;
                flags = FOptFlags.None;
                flagsMask = FOptFlags.Inverse;
                itemFlagsDefault = FOptItemFlags.None;
                itemFlagsMask = FOptItemFlags.None;
            }
        }

        public class MsgOutOption : FOpt
        {
            #region КлассыОпций

            public class TextOption : FOptText
            {
                private const string id_xml = "Message.Outgoing.Text";

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public TextOption(Dso filter, string id) : base(filter, id)
                {
                    is4Sql = false;
                    description = "текст отправленного сообщения";
                }
            }

            public class ByOption : FOptEmployee
            {
                private const string id_xml = "Message.Outgoing.By";

                public override bool RenderSqlPrepClause()
                {
                    return Enabled;
                }

                public override void RenderSqlWhereClause()
                {
                    ds.w.Write("TI.КодСотрудникаПолучателя");
                    if (Inverse) ds.w.Write(" NOT");
                    ds.w.Write(" IN (");
                    RenderCSVList();
                    ds.w.Write(")");
                }

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public ByOption(Dso filter, string id) : base(filter, id)
                {
                    is4Sql = false;
                    description = "cообщение отправлено сотруднику";
                }
            }

            public class DateOption : FOptDateTime
            {
                private const string id_xml = "Message.Outgoing.Date";

                protected override void RenderSqlLValue()
                {
                    ds.w.Write("TI.Отправлено");
                }

                internal override void GetIDs(string[] ids, ref int i)
                {
                    base.GetIDs(ids, ref i);
                    ids[i++] = id_xml;
                }

                public DateOption(Dso filter, string id) : base(filter, id)
                {
                    is4Sql = false;
                    description = "дата отправления сообщения";
                }
            }

            #endregion

            private bool renderText;
            private bool renderBy;
            private bool renderDate;

            private TextOption text;
            private ByOption by;
            private DateOption date;

            #region ACCESSORS

            public TextOption Text
            {
                get { return text; }
            }

            public ByOption By
            {
                get { return by; }
            }

            public DateOption Date
            {
                get { return date; }
            }

            #endregion

            public override bool RenderSqlPrepClause()
            {
                renderText = text.RenderSqlPrepClause();
                renderBy = by.RenderSqlPrepClause();
                renderDate = date.RenderSqlPrepClause();

                return renderText || renderBy || renderDate;
            }

            public override void RenderSqlWhereClause()
            {
                if (Inverse) ds.w.Write("NOT ");
                ds.w.Write("EXISTS (SELECT * FROM vwСообщенияИсходящие TI WHERE TI.КодДокумента=T0.КодДокумента");

                if (renderText)
                {
                    ds.w.Write(" AND (");
                    text.RenderSqlWhereClause();
                    ds.w.Write(")");
                }
                if (renderBy)
                {
                    ds.w.Write(" AND (");
                    by.RenderSqlWhereClause();
                    ds.w.Write(")");
                }
                if (renderDate)
                {
                    ds.w.Write(" AND (");
                    date.RenderSqlWhereClause();
                    ds.w.Write(")");
                }

                ds.w.Write(")");
            }

            public MsgOutOption(Dso filter, string id) : base(filter, id)
            {
                text = new TextOption(filter, id + "Text");
                by = new ByOption(filter, id + "By");
                date = new DateOption(filter, id + "Date");
                is4User = false;

                flags = FOptFlags.None;
                flagsMask = FOptFlags.Inverse;
                itemFlagsDefault = FOptItemFlags.None;
                itemFlagsMask = FOptItemFlags.None;
            }
        }

        #endregion

        protected IDsOption ids;
        protected SearchTextOption searchText;
        protected PersonOption person;
        protected TypeOption type;
        protected NumberOption number;
        protected DateOption date;
        protected DescriptionOption description;
        protected LinkedDocOption linkedDoc;
        protected EFormOption eform; //проверяет exist vwДокументыДанные 
        protected ImageOption image; //проверяет exist vwИзображенияДокументов
        protected SignOption sign; //проверяет exist ПодписиДокументов
        protected MsgInOption msgIn; //проверяет exist vwСообщенияВходящие 
        protected MsgOutOption msgOut; //проверяет exist vwСообщенияИсходящие  

        public bool includeDocumentNull;

        private FOpt[][] groups;
        private string[] groupNames;

        #region ACCESSORS

        public IDsOption IDs
        {
            get { return ids; }
        }

        public SearchTextOption SearchText
        {
            get { return searchText; }
        }

        public PersonOption Person
        {
            get { return person; }
        }

        public TypeOption Type
        {
            get { return type; }
        }

        public NumberOption Number
        {
            get { return number; }
        }

        public DateOption Date
        {
            get { return date; }
        }

        public DescriptionOption Description
        {
            get { return description; }
        }

        public LinkedDocOption LinkedDoc
        {
            get { return linkedDoc; }
        }

        public EFormOption EForm
        {
            get { return eform; }
        }

        public ImageOption Image
        {
            get { return image; }
        }

        public SignOption Sign
        {
            get { return sign; }
        }

        public MsgInOption MsgIn
        {
            get { return msgIn; }
        }

        public MsgOutOption MsgOut
        {
            get { return msgOut; }
        }

        public FOpt[][] Groups
        {
            get { return groups; }
        }

        public string[] GroupNames
        {
            get { return groupNames; }
        }

        #endregion

        protected virtual void InitOption()
        {
            ids = new IDsOption(this, "ids"); //"код документа" Patterns.ValueOption
            searchText = new SearchTextOption(this, "search"); //"документ" Patterns.ValueOption 
            person = new PersonOption(this, "person"); //"лица-контрагенты" Patterns.PersonListOption
            type = new TypeOption(this, "type"); //"тип документа" Patterns.ListOption
            number = new NumberOption(this, "number"); //"номер документа" Patterns.TextOption
            date = new DateOption(this, "date"); //"дата документа"  Patterns.DateOption
            description = new DescriptionOption(this, "description"); //"описание" Patterns.TextListOption
            linkedDoc = new LinkedDocOption(this, "linkedDoc");
                //"документ связан с другим документом" Patterns.ValueOption | Node

            eform = new EFormOption(this, "eform");
            image = new ImageOption(this, "image");
            sign = new SignOption(this, "sign");
            msgIn = new MsgInOption(this, "msgIn");
            msgOut = new MsgOutOption(this, "msgOut");
        }

        public DsoDoc() : base(Env.Docs)
        {
            InitOption();

            #region разделение опций по группам

            groupNames = new[]
                             {
                                 "Документ",
                                 "Изображение",
                                 "Эл.Форма",
                                 "Сообщения"
                             };

            groups = new[]
                         {
                             new FOpt[]
                                 {
                                     ids,
                                     searchText,
                                     person,
                                     type,
                                     number,
                                     date,
                                     description,
                                     linkedDoc,
                                 },

                             new FOpt[]
                                 {
                                     image.Scaned,
                                     image.Archive,
                                     image.Exists,
                                     image.NotExists
                                 },

                             new FOpt[]
                                 {
                                     eform.Exists,
                                     eform.NotExists,
                                     sign.Finished,
                                     sign.NotFinished,
                                     sign.SignedBy,
                                     sign.NotSignedBy,
                                     eform.ChangedBy,
                                 },

                             new FOpt[]
                                 {
                                     msgIn.By,
                                     msgIn.Date,
                                     msgIn.Text,
                                     msgIn.Read,
                                     msgIn.Unread,

                                     msgOut.By,
                                     msgOut.Date,
                                     msgOut.Text
                                 }
                         };

            #endregion

            PopulateOptsIDs();
        }

        protected override void RenderSqlSelectClause()
        {
            w.Write("SELECT");
            if (MaxRecords > 0) w.Write(" TOP {0}", StartRecord + MaxRecords);
            w.Write(" * FROM vwДокументы T0 (nolock) ");
        }

        public override int GetID(DataRow row)
        {
            return (int) row["КодДокумента"];
        }

        public override string GetName(DataRow row)
        {
            string _ret = "";
            if (GetID(row) == -1)
                _ret = "<Договор неуказан>";
            else
            {
                var doc = new Doc(GetID(row).ToString());
                _ret = doc.FullName;
            }
            return _ret;
        }

        public DataTable GetData()
        {
            var dt = new DataTable();
            Env.Docs.Find(GetSqlCommand().CommandText, dt);
            return dt;
        }

        protected void RenderForm(TextWriter w)
        {
            w.Write("<span id=\"{0}\">", "dsoDoc");
            RenderFormContent(w);
            w.Write("</span>");
        }

        protected void RenderFormContent(TextWriter w)
        {

        }
    }
}