using System;
using System.Xml;
using Kesco.Lib.Win.Data.DALC.Documents.Search.Patterns;

namespace Kesco.Lib.Win.Data.DALC.Documents.Search.Contract
{
    [Option("EForm.LinkedToByCuratorMe", typeof (LinkedToByCuratorMe))]
    public class LinkedToByCuratorMe : MyOption
    {
        protected LinkedToByCuratorMe(XmlElement el)
            : base(el)
        {
            shortTextPrefix = Resources.GetString("shortTextPrefix");
        }

        public override string GetSQL(bool throwOnError)
        {
            return
                @"EXISTS (SELECT TI.*
	FROM vw��������������� TI WITH(NOLOCK) INNER JOIN
	vw������������������������ TL WITH(NOLOCK) ON TI.��������������������� = TL.������������
		WHERE TL.���������������� IN (SELECT c.���������������� FROM dbo.�������������� c INNER JOIN
			dbo.�������������� p ON c.L >= p.L AND c.R <= p.R AND p.����������������=2039)
			AND TI.�����������������������=T0.������������ AND TL.�������������1 = " +
                Value + ")" + Environment.NewLine;
        }
    }
}
