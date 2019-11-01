using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    /// <summary>
    /// печать электронных форм
    /// </summary>
    public class PrintDataDALC : DALC
    {
        private const string printIDField = "“ипѕечати";
        private const string urlField = "URL";
        private const string docTypeField = " од“ипаƒокумента";
        private const string paperSizeField = "–азмерЅумаги";
        private const string fnPrintForm = "fn_ѕечатные‘ормы";

        public PrintDataDALC(string connectionString)
            : base(connectionString)
        {
            tableName = "ƒокументы.dbo.vwѕечатные‘ормы";
            idField = " одѕечатной‘ормы";
            nameField = "ѕечатна€‘орма";
        }

        #region Accessors

        public string URL
        {
            get { return urlField; }
        }

        public string DocTypeField
        {
            get { return docTypeField; }
        }

        public string PrintIDField
        {
            get { return printIDField; }
        }

        public string PaperSizeField
        {
            get { return paperSizeField; }
        }

        #endregion

        #region GetData

        public DataSet GetPrintForms()
        {
            var da = new SqlDataAdapter("SELECT " +
                                        idField + ", " +
                                        docTypeField + ", " +
                                        nameField +
                                        " FROM " + tableName +
                                        " ORDER BY " + nameField,
                                        new SqlConnection(connectionString));
            return CMD_FillDS(da);
        }

		public DataTable GetEFormPrintTypeData(int docTypeID, int docID)
		{
			return GetDataTable("SELECT " +
										idField + ", " +
										nameField + ", " +
										urlField + ", " +
										printIDField + ", " +
										paperSizeField +
										" FROM " + fnPrintForm + "( @DocID)",
						  delegate(SqlCommand cmd)
						  {
							  AddParam(cmd, "@DocID", SqlDbType.Int, docID);
						  }
			);
		}

        public bool CanPrintEForm(int docTypeID)
        {
            return FieldExists(" WHERE " + docTypeField + " = @DocTypeID",
                               delegate(SqlCommand cmd)
                                   {
                                       AddParam(cmd, "@DocTypeID", SqlDbType.Int, docTypeID);
                                   });
        }

        #endregion
    }
}