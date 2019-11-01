using System;
using System.Collections;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Document
{
    [Option("ЛицаКонтрагенты", typeof (ЛицаКонтрагенты))]
    public class ЛицаКонтрагенты : PersonListOption
    {
        protected ЛицаКонтрагенты(XmlElement el) : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPrefix2 = Resources.GetString("htmlPrefix2");
            if ((int) Mode > 2)
            {
                htmlPrefix += Resources.GetString("htmlPrefix3");
                htmlPrefix2 += Resources.GetString("htmlPrefix3");
                shortTextPrefix += Resources.GetString("htmlPrefix3");
            }
            htmlPostfix = "";

            textItemPrefix = "[";
            textItemPostfix = "]";
        }

        public override string GetHTML()
        {
            if ((int) Mode <= 2)
                return base.GetHTML();

            var values = new ArrayList();
            var resources = new ResourceManager(typeof (ЛицаКонтрагенты));
            if (Modes.PersonOrganization == (Mode & Modes.PersonOrganization))
                values.Add(resources.GetString("PersonOrganization"));

            if (Modes.PersonPhysical == (Mode & Modes.PersonPhysical))
            {
                if (values.Count > 0)
                    values.Add(", ");
                values.Add(resources.GetString("PersonPhysical"));
            }

            if (Modes.PersonBank == (Mode & Modes.PersonBank))
            {
                if (values.Count > 0)
                    values.Add(", ");
                values.Add(resources.GetString("PersonBank"));
            }

            if (values.Count > 0)
            {
                values.Insert(0, resources.GetString("PersonType"));
                values.Add(". ");
            }

            if (Modes.PersonUncheck == (Mode & Modes.PersonUncheck))
            {
                values.Add(resources.GetString("PersonUncheck") + ". ");
            }

            if (Modes.PersonCheck == (Mode & Modes.PersonCheck))
            {
                values.Add(resources.GetString("PersonCheck") + ". ");
            }

            if (Modes.PersonArea == (Mode & Modes.PersonArea))
            {
                values.Add(resources.GetString("PersonArea") + NameArea + "\". ");
            }

            if (Modes.PersonOPForma == (Mode & Modes.PersonOPForma))
            {
                values.Add(resources.GetString("PersonOPForma") + NameOPForm + "\". ");
            }

            if (Modes.PersonUsers == (Mode & Modes.PersonUsers))
            {
                values.Add(resources.GetString("PersonUsers"));
                int i = 0;
                foreach (String user in ListUsers.Values)
                {
                    i++;
                    values.Add(user);
                    values.Add(i < ListUsers.Count ? ", " : ". ");
                }
            }

            if (Modes.PersonThemes == (Mode & Modes.PersonThemes))
            {
                if (ListTypes.Values.Count > 0)
                {
                    values.Add(resources.GetString("PersonThemes"));
                    if (Modes.PersonSubThemes == (Mode & Modes.PersonSubThemes))
                    {
                        values.Add(resources.GetString("PersonSubThemes"));
                    }
                    values.Add(": ");
                    int i = 0;
                    foreach (String type in ListTypes.Values)
                    {
                        i++;
                        values.Add(type);
                        values.Add(i < ListTypes.Count ? ", " : ". ");
                    }
                }
                else
                    values.Add(resources.GetString("PersonThemesNull"));
            }

            if (Modes.PersonAnyBusinessProject == (Mode & Modes.PersonAnyBusinessProject))
            {
                values.Add(resources.GetString("PersonAnyBusinessProject"));
            }
            else if (Modes.PersonBusinessProject == (Mode & Modes.PersonBusinessProject))
            {
                values.Add(resources.GetString("PersonBusinessProject"));
                if (Modes.PersonSubBusinessProject == (Mode & Modes.PersonSubBusinessProject))
                {
                    values.Add(resources.GetString("PersonSubBusinessProject"));
                }
                values.Add(": " + NameBusinessProject + ". ");
            }
            else if (Modes.PersonNullBusinessProject == (Mode & Modes.PersonNullBusinessProject))
            {
                values.Add(resources.GetString("PersonNullBusinessProject"));
            }

            string s = "";
            s = values.Count == 0 ? emptyValueText : values.Cast<object>().Aggregate(s, (current, t) => current + (htmlItemPrefix + t + htmlItemPostfix));

            return Regex.Replace((values.Count > 1 ? htmlPrefix2 : htmlPrefix), "[ ]$", ": ") +
                   htmlLinkPrefix +
                   s +
                   htmlLinkPostfix +
                   htmlPostfix;
        }

        public override string GetSQL(bool throwOnError)
        {
            try
            {
                string[] vals = GetValues(throwOnError);
                if (vals.Length == 0 && 2 >= (int) Mode)
                    throw new Exception(Resources.GetString("GetSQL"));

                if ((int) Mode <= 2)
                {
                    return
                        GetSQLCondition(
                            @"EXISTS (SELECT *
							FROM Документы.dbo.vwЛицаДокументов TI WITH(NOLOCK)
							WHERE TI.КодДокумента=T0.КодДокумента AND TI.КодЛица = @VAL)");
                }

                string PersonType = "";
                string PersonOPF = "";
                bool isAND = false;

                if (Modes.PersonOrganization == (Mode & Modes.PersonOrganization))
                {
                    isAND = true;
                    PersonType = " T2.ТипЛица=1 ";
                }

                if (Modes.PersonPhysical == (Mode & Modes.PersonPhysical))
                {
                    PersonType += isAND ? " OR " : "";
                    PersonType += " T2.ТипЛица=2 ";
                    isAND = true;
                }
                if (Modes.PersonBank == (Mode & Modes.PersonBank))
                {
                    PersonType += isAND ? " OR " : "";
                    PersonType += " (T2.БИК<>'' OR T2.SWIFT<>'') ";
                    isAND = true;
                }
                if (PersonType != "")
                {
                    PersonType += ") ";
                    PersonType = " (" + PersonType;
                }

                //проверка бухгалтерией
                if (
                    !(Modes.PersonUncheck == (Mode & Modes.PersonUncheck) &&
                      Modes.PersonCheck == (Mode & Modes.PersonCheck)))
                {
                    if (Modes.PersonUncheck == (Mode & Modes.PersonUncheck))
                    {
                        PersonType += isAND ? " AND " : "";
                        PersonType += " T2.Проверено=0 ";
                        isAND = true;
                    }
                    if (Modes.PersonCheck == (Mode & Modes.PersonCheck))
                    {
                        PersonType += isAND ? " AND " : "";
                        PersonType += " T2.Проверено=1 ";
                        isAND = true;
                    }
                }

                //страна регистрации
                if (Modes.PersonArea == (Mode & Modes.PersonArea))
                {
                    PersonType += isAND ? " AND " : "";
                    PersonType += " T2.КодТерритории='" + IDArea + "'";
                    isAND = true;
                }

                //ОргПравФорма
                if (Modes.PersonOPForma == (Mode & Modes.PersonOPForma))
                {
                    PersonOPF = "";
                    PersonType += isAND ? " AND " : "";
                    PersonType +=
                        "(EXISTS (SELECT Л1.КодЛица FROM Справочники.dbo.vwКарточкиФизЛиц Л1 INNER JOIN Справочники.dbo.ОргПравФормы Л4 ON Л4.КодОргПравФормы = Л1.КодОргПравФормы WHERE T2.КодЛица = Л1.КодЛица AND Л4.КодОргПравФормы = " +
                        IDOPForm + " " +
                        "UNION SELECT Л2.КодЛица FROM Справочники.dbo.vwКарточкиЮрЛиц Л2 INNER JOIN Справочники.dbo.ОргПравФормы Л3 ON Л3.КодОргПравФормы = Л2.КодОргПравФормы WHERE T2.КодЛица = Л2.КодЛица AND Л3.КодОргПравФормы = " +
                        IDOPForm + ")) ";

                    isAND = true;
                }

                //ответственные сотрудники
                if (Modes.PersonUsers == (Mode & Modes.PersonUsers))
                {
                    string users = "";
                    int n = 0;
                    foreach (String user in ListUsers.Keys)
                    {
                        n++;
                        users += "'" + user + "'";
                        if (n < ListUsers.Count)
                            users += ", ";
                    }
                    PersonType += isAND ? " AND " : "";
                    PersonType +=
                        " EXISTS (SELECT * FROM Справочники.dbo.vwЛица_Сотрудники ЛС WHERE T2.КодЛица = ЛС.КодЛица AND ЛС.КодСотрудника IN (" +
                        users + ")) ";

                    isAND = true;
                }
                //по типам
                if (Modes.PersonThemes == (Mode & Modes.PersonThemes))
                {
                    if (ListTypes.Keys.Count > 0)
                    {
                        string types = "";
                        int n = 0;
                        foreach (String type in ListTypes.Keys)
                        {
                            n++;
                            types += "'" + type + "'";
                            if (n < ListTypes.Count)
                                types += ", ";
                        }
                        if (Modes.PersonSubThemes == (Mode & Modes.PersonSubThemes))
                            types =
                                "SELECT Все.КодТемыЛица FROM Справочники.dbo.vwТемыЛиц Все INNER JOIN (SELECT L, R FROM Справочники.dbo.vwТемыЛиц WHERE КодТемыЛица IN (" +
                                types + ")) Даны ON Все.L >= Даны.L AND Все.R <= Даны.R";

                        PersonType += isAND ? " AND " : "";
                        PersonType +=
                            " EXISTS (SELECT * FROM (SELECT ЛТЛ.*, ТЛ.КодТемыЛица FROM Справочники.dbo.Лица_ТипыЛиц ЛТЛ INNER JOIN Справочники.dbo.ТипыЛиц ТЛ ON  ЛТЛ.КодТипаЛица=ТЛ.КодТипаЛица WHERE Сотрудник = 0) ЛТ WHERE T2.КодЛица = ЛТ.КодЛица AND ЛТ.КодТемыЛица IN (" +
                            types + ") ) ";

                        isAND = true;
                    }
                    else
                    {
                        PersonType += isAND ? " AND " : "";
                        PersonType +=
                            " NOT EXISTS (SELECT * FROM (SELECT ЛТЛ.*, ТЛ.КодТемыЛица FROM Справочники.dbo.Лица_ТипыЛиц ЛТЛ INNER JOIN Справочники.dbo.ТипыЛиц ТЛ ON  ЛТЛ.КодТипаЛица=ТЛ.КодТипаЛица WHERE Сотрудник = 0) ЛТ WHERE T2.КодЛица = ЛТ.КодЛица ) ";
                        // AND ЛТ.КодЛица_ТипыЛиц IS NULL

                        isAND = true;
                    }

                }
                //бизнес проекты
                if (Modes.PersonAnyBusinessProject == (Mode & Modes.PersonAnyBusinessProject))
                {
                    PersonType += isAND ? " AND " : "";
                    PersonType += " T2.КодБизнесПроекта IS NOT NULL ";
                    isAND = true;
                }
                else if (Modes.PersonBusinessProject == (Mode & Modes.PersonBusinessProject))
                {
                    int id = IDBusinessProject;
                    PersonType += isAND ? " AND " : "";

                    if (Modes.PersonSubBusinessProject == (Mode & Modes.PersonSubBusinessProject))
                    {
                        PersonType +=
                            " T2.КодБизнесПроекта IN (SELECT ВсеБизнесПроекты.КодБизнесПроекта FROM	Справочники.dbo.БизнесПроекты ВсеБизнесПроекты " +
                            "INNER JOIN (SELECT L, R FROM Справочники.dbo.БизнесПроекты WHERE КодБизнесПроекта= '" +
                            id.ToString() + "') ДанБизнесПроект " +
                            "ON ВсеБизнесПроекты.L >= ДанБизнесПроект.L AND ВсеБизнесПроекты.R <= ДанБизнесПроект.R) ";
                    }
                    else
                        PersonType += " T2.КодБизнесПроекта IS NOT NULL AND T2.КодБизнесПроекта= '" + id.ToString() +
                                      "'";

                    isAND = true;

                }
                else if (Modes.PersonNullBusinessProject == (Mode & Modes.PersonNullBusinessProject))
                {
                    PersonType += isAND ? " AND " : "";
                    PersonType += " T2.КодБизнесПроекта IS NULL ";
                    isAND = true;
                }

                return PersonType == ""
                           ? null
                           : "EXISTS (SELECT * FROM Документы.dbo.vwЛицаДокументов TI WITH(NOLOCK) " +
                             "WHERE TI.КодДокумента=T0.КодДокумента AND " +
                             "EXISTS (SELECT * FROM Справочники.dbo.vwЛица T2 WITH(NOLOCK) " + PersonOPF +
                             " WHERE T2.КодЛица=TI.КодЛица AND " +
                             PersonType + " ))";
            }
            catch (Exception ex)
            {
                if (throwOnError)
                    throw ex;
                return null;
            }
        }
    }
}
