namespace Kesco.Lib.Win.Data.DALC.Documents
{
    public class ErrorMessageDALC : DALC
    {
        private const string nameFieldEng = "Message";

        public ErrorMessageDALC(string connectionString) : base(connectionString)
        {
            tableName = "dbo.ErrorMessages";

            idField = "КодОшибки";
            nameField = "Сообщение";
        }

        public string NameFieldEng
        {
            get { return nameFieldEng; }
        }
    }
}
