using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    /// <summary>
    /// DAL-компонент для доступа к таблице Документы.dbo.ДокументыДанные
    /// </summary>
    public class DocDataDALC : DALC
    {
        protected string personIDNameField = "КодЛица";
        protected string firstPersonIDNameField = "КодЛица1";
        protected string secondPersonIDNameField = "КодЛица2";
        protected string thirdPersonIDNameField = "КодЛица3";
        protected string fourthPersonIDNameField = "КодЛица4";

        public DocDataDALC(string connectionString)
            : base(connectionString)
        {
            tableName = "Документы.dbo.vwДокументыДанные";

            idField = "КодДокумента";
            nameField = "";
        }

        #region Accessors

        public string PersonIDNameField
        {
            get { return personIDNameField; }
        }

        public string FirstPersonIDNameField
        {
            get { return firstPersonIDNameField; }
        }

        public string SecondPersonIDNameField
        {
            get { return secondPersonIDNameField; }
        }

        public string ThirdPersonIDNameField
        {
            get { return thirdPersonIDNameField; }
        }

        public string FourthPersonIDNameField
        {
            get { return fourthPersonIDNameField; }
        }

        #endregion

        #region Get Data

        public bool IsDataPresent(int id)
        {
            return FieldExists(id);
        }

		public string GetDocumentPersons(int docID, CancellationToken ct)
        {
            if (docID < 1)
                return "";

            var obj = GetRecord<string>(
                "SELECT ISNULL(',' + CONVERT(varchar," + firstPersonIDNameField +
                "),'') + ISNULL(',' + CONVERT(varchar," + secondPersonIDNameField +
                "),'') + ISNULL(',' + CONVERT(varchar," + thirdPersonIDNameField +
                "),'') + ISNULL(',' + CONVERT(varchar," + fourthPersonIDNameField + "),'') " + personIDNameField +
                " FROM " + tableName +
                " WHERE " + idField + " = @ID",
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@ID", SqlDbType.Int, docID);
                    },
                dr => !dr.IsDBNull(0) ? dr[0].ToString() : "", ct);

            return (obj ?? "");
        }

        #endregion
    }
}