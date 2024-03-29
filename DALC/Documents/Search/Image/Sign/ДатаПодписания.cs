using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Image.Sign
{
    /// <summary>
    /// Summary description for ��������������.
    /// </summary>
    [Option("Image.Sing.��������������", typeof (��������������))]
	[SeparateOption("Image.Sing.��������������", typeof(�����������))]
	public class �������������� : DateOption
    {
        protected ��������������(XmlElement el) : base(el)
        {
            shortTextPrefix = htmlPrefix = Resources.GetString("shortTextPrefix");
            shortTextPostfix = htmlPostfix = "";
        }

        public override string GetSQL(bool throwOnError)
        {
            string sql;
            try
            {
                sql =
					@"
EXISTS (SELECT     *
FROM	����������������� TI WITH(NOLOCK)
WHERE TI.������������=T0.������������ AND TI.����������<>101 AND " + (IsSeparate() ? "TI.����������������������� IS NOT NULL AND ":"") +
                    GetSQLCondition("TI.����") + ")";
            }
            catch (Exception ex)
            {
                if (throwOnError)
                    throw ex;
                return null;
            }
            return sql;
        }
    }
}
