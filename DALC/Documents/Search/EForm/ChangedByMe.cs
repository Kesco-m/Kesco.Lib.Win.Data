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
                FROM Документы.dbo.vwДокументыДанные AS T1 WITH(NOLOCK)
                INNER JOIN Инвентаризация.dbo.Сотрудники AS T2 WITH(NOLOCK) ON T1.Изменил = T2.КодСотрудника
                WHERE T1.КодДокумента=T0.КодДокумента AND T1.Изменил=" +
                Value + ")" + Environment.NewLine;
        }
    }
}
