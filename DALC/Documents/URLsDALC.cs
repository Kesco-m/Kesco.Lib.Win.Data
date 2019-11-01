namespace Kesco.Lib.Win.Data.DALC.Documents
{
    /// <summary>
    /// Summary description for URLsDALC.
    /// </summary>
    public class URLsDALC : DALC
    {
        public enum URLsCode
        {
            HelpURL = 1,
            ChangesURL,
            PrintTemplate,
            CreateDocsLinkURL = 5,
            ScalaExportURL = 8,
            ICMFExportURL,
            ICExportURL,
            EFormCreateURL,
            ReportingServiceURL,
            AnswerFormUrl,
			CreateSlaveUrl,
            SearchPersonURL = 21,
            SearchEmployeeURL,
            CreateTransactionURL = 31,
            ShowTransactionURL,
            CreateContractURL = 41,
            ShowPersonUrl = 50,
            CreateClientURL = 51,
            CreateClientPersonURL,
            UsersURL = 60,
            DialUrl = 61
        }

        public URLsDALC(string connectionString) : base(connectionString)
        {
            tableName = "URLs";

            idField = " Ó‰URL";
            nameField = "URL";
        }
    }
}
