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
	FROM vwСвязиДокументов TI WITH(NOLOCK) INNER JOIN
	vwДокументыДокументыДанные TL WITH(NOLOCK) ON TI.КодДокументаОснования = TL.КодДокумента
		WHERE TL.КодТипаДокумента IN (SELECT c.КодТипаДокумента FROM dbo.ТипыДокументов c INNER JOIN
			dbo.ТипыДокументов p ON c.L >= p.L AND c.R <= p.R AND p.КодТипаДокумента=2039)
			AND TI.КодДокументаВытекающего=T0.КодДокумента AND TL.КодСотрудника1 = " +
                Value + ")" + Environment.NewLine;
        }
    }
}
