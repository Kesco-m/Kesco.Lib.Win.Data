using System;
using System.Collections;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Document
{
    [Option("���������������", typeof (���������������))]
    public class ��������������� : PersonListOption
    {
        protected ���������������(XmlElement el) : base(el)
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
            var resources = new ResourceManager(typeof (���������������));
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
							FROM ���������.dbo.vw�������������� TI WITH(NOLOCK)
							WHERE TI.������������=T0.������������ AND TI.������� = @VAL)");
                }

                string PersonType = "";
                string PersonOPF = "";
                bool isAND = false;

                if (Modes.PersonOrganization == (Mode & Modes.PersonOrganization))
                {
                    isAND = true;
                    PersonType = " T2.�������=1 ";
                }

                if (Modes.PersonPhysical == (Mode & Modes.PersonPhysical))
                {
                    PersonType += isAND ? " OR " : "";
                    PersonType += " T2.�������=2 ";
                    isAND = true;
                }
                if (Modes.PersonBank == (Mode & Modes.PersonBank))
                {
                    PersonType += isAND ? " OR " : "";
                    PersonType += " (T2.���<>'' OR T2.SWIFT<>'') ";
                    isAND = true;
                }
                if (PersonType != "")
                {
                    PersonType += ") ";
                    PersonType = " (" + PersonType;
                }

                //�������� ������������
                if (
                    !(Modes.PersonUncheck == (Mode & Modes.PersonUncheck) &&
                      Modes.PersonCheck == (Mode & Modes.PersonCheck)))
                {
                    if (Modes.PersonUncheck == (Mode & Modes.PersonUncheck))
                    {
                        PersonType += isAND ? " AND " : "";
                        PersonType += " T2.���������=0 ";
                        isAND = true;
                    }
                    if (Modes.PersonCheck == (Mode & Modes.PersonCheck))
                    {
                        PersonType += isAND ? " AND " : "";
                        PersonType += " T2.���������=1 ";
                        isAND = true;
                    }
                }

                //������ �����������
                if (Modes.PersonArea == (Mode & Modes.PersonArea))
                {
                    PersonType += isAND ? " AND " : "";
                    PersonType += " T2.�������������='" + IDArea + "'";
                    isAND = true;
                }

                //������������
                if (Modes.PersonOPForma == (Mode & Modes.PersonOPForma))
                {
                    PersonOPF = "";
                    PersonType += isAND ? " AND " : "";
                    PersonType +=
                        "(EXISTS (SELECT �1.������� FROM �����������.dbo.vw�������������� �1 INNER JOIN �����������.dbo.������������ �4 ON �4.��������������� = �1.��������������� WHERE T2.������� = �1.������� AND �4.��������������� = " +
                        IDOPForm + " " +
                        "UNION SELECT �2.������� FROM �����������.dbo.vw������������� �2 INNER JOIN �����������.dbo.������������ �3 ON �3.��������������� = �2.��������������� WHERE T2.������� = �2.������� AND �3.��������������� = " +
                        IDOPForm + ")) ";

                    isAND = true;
                }

                //������������� ����������
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
                        " EXISTS (SELECT * FROM �����������.dbo.vw����_���������� �� WHERE T2.������� = ��.������� AND ��.������������� IN (" +
                        users + ")) ";

                    isAND = true;
                }
                //�� �����
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
                                "SELECT ���.����������� FROM �����������.dbo.vw������� ��� INNER JOIN (SELECT L, R FROM �����������.dbo.vw������� WHERE ����������� IN (" +
                                types + ")) ���� ON ���.L >= ����.L AND ���.R <= ����.R";

                        PersonType += isAND ? " AND " : "";
                        PersonType +=
                            " EXISTS (SELECT * FROM (SELECT ���.*, ��.����������� FROM �����������.dbo.����_������� ��� INNER JOIN �����������.dbo.������� �� ON  ���.�����������=��.����������� WHERE ��������� = 0) �� WHERE T2.������� = ��.������� AND ��.����������� IN (" +
                            types + ") ) ";

                        isAND = true;
                    }
                    else
                    {
                        PersonType += isAND ? " AND " : "";
                        PersonType +=
                            " NOT EXISTS (SELECT * FROM (SELECT ���.*, ��.����������� FROM �����������.dbo.����_������� ��� INNER JOIN �����������.dbo.������� �� ON  ���.�����������=��.����������� WHERE ��������� = 0) �� WHERE T2.������� = ��.������� ) ";
                        // AND ��.�������_������� IS NULL

                        isAND = true;
                    }

                }
                //������ �������
                if (Modes.PersonAnyBusinessProject == (Mode & Modes.PersonAnyBusinessProject))
                {
                    PersonType += isAND ? " AND " : "";
                    PersonType += " T2.���������������� IS NOT NULL ";
                    isAND = true;
                }
                else if (Modes.PersonBusinessProject == (Mode & Modes.PersonBusinessProject))
                {
                    int id = IDBusinessProject;
                    PersonType += isAND ? " AND " : "";

                    if (Modes.PersonSubBusinessProject == (Mode & Modes.PersonSubBusinessProject))
                    {
                        PersonType +=
                            " T2.���������������� IN (SELECT ����������������.���������������� FROM	�����������.dbo.������������� ���������������� " +
                            "INNER JOIN (SELECT L, R FROM �����������.dbo.������������� WHERE ����������������= '" +
                            id.ToString() + "') ��������������� " +
                            "ON ����������������.L >= ���������������.L AND ����������������.R <= ���������������.R) ";
                    }
                    else
                        PersonType += " T2.���������������� IS NOT NULL AND T2.����������������= '" + id.ToString() +
                                      "'";

                    isAND = true;

                }
                else if (Modes.PersonNullBusinessProject == (Mode & Modes.PersonNullBusinessProject))
                {
                    PersonType += isAND ? " AND " : "";
                    PersonType += " T2.���������������� IS NULL ";
                    isAND = true;
                }

                return PersonType == ""
                           ? null
                           : "EXISTS (SELECT * FROM ���������.dbo.vw�������������� TI WITH(NOLOCK) " +
                             "WHERE TI.������������=T0.������������ AND " +
                             "EXISTS (SELECT * FROM �����������.dbo.vw���� T2 WITH(NOLOCK) " + PersonOPF +
                             " WHERE T2.�������=TI.������� AND " +
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
