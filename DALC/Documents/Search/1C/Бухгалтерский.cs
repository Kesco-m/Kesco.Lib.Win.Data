using System.Xml;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search._1C
{
    /// <summary>
    /// ����� �� ������������� ����������.
    /// </summary>
    [Option("�������������", typeof (�������������))]
    public class ������������� : Option
    {
        protected �������������(XmlElement el) : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = "";

            htmlPrefix = Resources.GetString("htmlPrefix");
            htmlPostfix = "";
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"
	            EXISTS (SELECT *
	            FROM ���������.dbo.�������������� TI
	            WHERE TI.����������������=T0.���������������� AND (������������� = 1 OR ����������������������� > 0))
	            ";
        }
    }
}
