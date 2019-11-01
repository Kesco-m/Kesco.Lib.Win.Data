using System;
using System.Threading;
using System.Xml;
using Kesco.Lib.Win.Data.Business.V2.Docs.DomainObjects;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;
using Kesco.Lib.Win.Data.Types;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Links
{
    /*
		�������: ��������
		�����:	 ������ � ������ ����������
		
		��������:
		
		������� � ������ � ����������: [�������]
		
		�������� �������� �� ���������: [������� �555 �� 01.01.01]
		�������� ��������������� �������� �� ���������: [������� �555 �� 01.01.01]
		�������� �������� ���������� ���������: [������� �555 �� 01.01.01]
		�������� �������� ���������������� ���������� ���������: [������� �555 �� 01.01.01]
	
	 */

    [Option("�����������������", typeof (�����������������))]
    public class ����������������� : ValueOption
    {
        private bool osnovaniya = true;
        private bool osnovaniyaAll;
        private bool vytekayuschie = true;
        private bool vytekayuschieAll;
        private Doc doc;
        private int docID;

        protected �����������������(XmlElement el) : base(el)
        {

            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPostfix = "";

            textItemPrefix = "[";
            textItemPostfix = "]";

            int id = KInt.FromXmlString(el.GetAttribute("ID"));
            docID = id;

            osnovaniya = KBoolean.FromXmlString(el.GetAttribute("Osnovaniya"), true);
            osnovaniyaAll = KBoolean.FromXmlString(el.GetAttribute("OsnovaniyaAll"), false);
            vytekayuschie = KBoolean.FromXmlString(el.GetAttribute("Vytekayuschie"), true);
            vytekayuschieAll = KBoolean.FromXmlString(el.GetAttribute("VytekayuschieAll"), false);

        }

        #region ACCESSORS

        public bool Osnovaniya
        {
            get { return osnovaniya; }
            set
            {
                osnovaniya = value;
                el.SetAttribute("Osnovaniya", KBoolean.ToXmlString(osnovaniya));

            }
        }

        public bool OsnovaniyaAll
        {
            get { return osnovaniyaAll; }
            set
            {
                osnovaniyaAll = value;
                el.SetAttribute("OsnovaniyaAll", KBoolean.ToXmlString(osnovaniyaAll));
            }
        }

        public bool Vytekayuschie
        {
            get { return vytekayuschie; }
            set
            {
                vytekayuschie = value;
                el.SetAttribute("Vytekayuschie", KBoolean.ToXmlString(vytekayuschie));
            }
        }

        public bool VytekayuschieAll
        {
            get { return vytekayuschieAll; }
            set
            {
                vytekayuschieAll = value;
                el.SetAttribute("VytekayuschieAll", KBoolean.ToXmlString(vytekayuschieAll));

            }
        }

        /// <summary>
        /// ��� ���������, � ������� ������ �����
        /// </summary>
        public int DocumentID
        {
            get { return docID; }
            set
            {
                docID = value;
                el.SetAttribute("ID", docID <= 0 ? "" : KInt.ToXmlString(docID));
            }
        }

        #endregion

        public override string GetHTML()
        {
            string s = Resources.GetString("Document");

            if (Osnovaniya)
            {
                if (OsnovaniyaAll) s += Resources.GetString("GetHTML.Message1");
                else s += Resources.GetString("GetHTML.Message2");
            }
            if (Osnovaniya && Vytekayuschie) s += Resources.GetString("Or");
            if (Vytekayuschie)
            {
                if (VytekayuschieAll) s += Resources.GetString("GetHTML.Message3");
                else s += Resources.GetString("GetHTML.Message4");
            }
            s += Resources.GetString("GetHTML.Message5") + " [<A href=#" + Meta.Name + ">";
            if (docID <= 0)
            {
                s += emptyValueText;
            }
            else
            {
                if (doc == null || doc.ID != docID)
                    doc = new Doc(docID.ToString());
                s += ((Thread.CurrentThread.CurrentUICulture.Name.StartsWith("ru") ? doc.FullName : doc.FullNameEng));
            }
            s += "</A>]";

            return s;
        }


        public override string GetShortText()
        {
            string s = "";

            if (Osnovaniya)
            {
                if (OsnovaniyaAll)
                    s += Resources.GetString("GetShortText.Message1");
                else
                    s += Resources.GetString("GetShortText.Message2") + Resources.GetString("GetShortText.Message1");
            }
            if (Osnovaniya && Vytekayuschie)
                s += Resources.GetString("Or") + " ";
            if (Vytekayuschie)
            {
                if (VytekayuschieAll)
                    s += Resources.GetString("GetShortText.Message3");
                else
                    s += Resources.GetString("GetShortText.Message2") + Resources.GetString("GetShortText.Message3");
            }
            s += ": ";
            if (docID <= 0)
            {
                s += emptyValueText;
            }
            else
            {
                if (doc == null || doc.ID != docID)
                    doc = new Doc(docID.ToString());
                s += ((Thread.CurrentThread.CurrentUICulture.Name.StartsWith("ru") ? doc.FullName : doc.FullNameEng));
            }
            s += "";

            return s;
        }

        public override string GetSQL(bool throwOnError)
        {
            if (docID < 1)
                if (throwOnError)
                    throw new Exception(Resources.GetString("GetSQL.Message1"));
                else
                    return null;
            if ((!Osnovaniya) && (!Vytekayuschie) && throwOnError)
                throw new Exception(Resources.GetString("GetSQL.Message2"));

            string s = "";
            if (Osnovaniya)
            {
                if (OsnovaniyaAll)
                    s += "EXISTS (SELECT * FROM ���������.dbo.fn_������������(" + docID.ToString() +
                         ") TI WHERE TI.������������=T0.������������)";
                else
                    s +=
                        "EXISTS (SELECT * FROM ���������.dbo.vw��������������� TI WITH(NOLOCK) WHERE TI.�����������������������=" +
                        docID.ToString() + " AND TI.���������������������=T0.������������)";
            }
            if (Osnovaniya && Vytekayuschie)
                s += " OR ";
            if (Vytekayuschie)
            {
                if (VytekayuschieAll)
                    s += "EXISTS (SELECT * FROM ���������.dbo.fn_�������������(" + docID.ToString() +
                         ") TI WHERE TI.������������=T0.������������)";
                else
                    s +=
                        "EXISTS (SELECT * FROM ���������.dbo.vw��������������� TI WITH(NOLOCK) WHERE TI.���������������������=" +
                        docID.ToString() + " AND TI.�����������������������=T0.������������)";
            }

            return s;
        }
    }
}
