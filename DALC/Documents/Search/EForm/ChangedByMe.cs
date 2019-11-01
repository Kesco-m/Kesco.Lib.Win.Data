using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.EForm
{
    [Option("EForm.ChangedByMe", typeof (ChangedByMe))]
    public class ChangedByMe : MyOption
    {
        protected ChangedByMe(XmlElement el) : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"
                EXISTS (SELECT *
                FROM ���������.dbo.vw��������������� AS T1 WITH(NOLOCK)
                INNER JOIN ��������������.dbo.���������� AS T2 WITH(NOLOCK) ON T1.������� = T2.�������������
                WHERE T1.������������=T0.������������ AND T1.�������=" +
                Value + ")" + Environment.NewLine;
        }
    }
}
