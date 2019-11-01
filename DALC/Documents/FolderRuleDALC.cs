using System;
using System.Data;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    #region Rule

    public class Rule
    {
        public int ID;
        public int FolderID;
        public Byte Mode;
        public int SenderID;
        public int AddresseeID;
        public int SignerID;
        public int DocTypeID;
        public int PersonID;
        public int DocID;
        public string Name;

        public Rule()
        {
        }

        public Rule(DataRow row) : this()
        {
            var folderRuleData = new FolderRuleDALC(null);
            object obj;

            obj = row[folderRuleData.ShowModeField];
            if (obj is Byte)
                Mode = (Byte) obj;

            obj = row[folderRuleData.IDField];
            if (obj is int)
                ID = (int) obj;

            obj = row[folderRuleData.DocIDField];
            if (obj is int)
                DocID = (int) obj;

            obj = row[folderRuleData.DocTypeIDField];
            if (obj is int)
                DocTypeID = (int) obj;

            obj = row[folderRuleData.FolderIDField];
            if (obj is int)
                FolderID = (int) obj;

            obj = row[folderRuleData.SenderIDField];
            if (obj is int)
                SenderID = (int) obj;

            obj = row[folderRuleData.AddresseeIDField];
            if (obj is int)
                AddresseeID = (int) obj;

            obj = row[folderRuleData.SignerIDField];
            if (obj is int)
                SignerID = (int) obj;

            obj = row[folderRuleData.PersonIDField];
            if (obj is int)
                PersonID = (int) obj;

            obj = row[folderRuleData.NameField];
            if (obj is string)
                Name = (string) obj;
        }
    }

    #endregion

    public class FolderRuleDALC : DALC
    {
        private const string folderIDField = "КодПапкиДокументов";
        private const string showModeField = "Режим";
        private const string senderIDField = "КодСотрудникаОтправителя";
        private const string addresseeIDField = "КодСотрудникаПолучателя";
        private const string signerIDField = "КодСотрудникаПодписавшего";
        private const string docTypeIDField = "КодТипаДокумента";
        private const string personIDField = "КодЛица";
        private const string docIDField = "КодДокумента";

        public FolderRuleDALC(string connectionString) : base(connectionString)
        {
            tableName = "Документы.dbo.vwПравилаПапкиДокументов";

            idField = "КодПравилаПапкиДокументов";
            nameField = "Описание";
        }

        #region Accessors

        public string FolderIDField
        {
            get { return folderIDField; }
        }

        public string ShowModeField
        {
            get { return showModeField; }
        }

        public string SenderIDField
        {
            get { return senderIDField; }
        }

        public string AddresseeIDField
        {
            get { return addresseeIDField; }
        }

        public string SignerIDField
        {
            get { return signerIDField; }
        }

        public string DocTypeIDField
        {
            get { return docTypeIDField; }
        }

        public string PersonIDField
        {
            get { return personIDField; }
        }

        public string DocIDField
        {
            get { return docIDField; }
        }

        #endregion

        public DataTable GetFolderRules(int folderID)
        {
            var cmd = new SqlDataAdapter(
                "SELECT " +
                idField + ", " +
                folderIDField + ", " +
                showModeField + ", " +
                senderIDField + ", " +
                addresseeIDField + ", " +
                signerIDField + ", " +
                docTypeIDField + ", " +
                personIDField + ", " +
                docIDField + ", " +
                nameField +
                " FROM " + tableName +
                " WHERE " + folderIDField + " = @ID" +
                " ORDER BY " + nameField,
                new SqlConnection(connectionString));

            AddParam(cmd.SelectCommand, "@ID", SqlDbType.Int, folderID);

            return (CMD_FillDT(cmd.SelectCommand, "FolderRules"));
        }

        public int SaveRule(Rule rule)
        {
            string queryString = "";

            if (rule.ID != 0)
                queryString =
                    "UPDATE " + tableName +
                    " SET " +
                    showModeField + " = " + ((rule.Mode != 0) ? rule.Mode.ToString() : "null") + ", " +
                    senderIDField + " = " + ((rule.SenderID != 0) ? rule.SenderID.ToString() : "null") + ", " +
                    addresseeIDField + " = " + ((rule.AddresseeID != 0) ? rule.AddresseeID.ToString() : "null") + ", " +
                    signerIDField + " = " + ((rule.SignerID != 0) ? rule.SignerID.ToString() : "null") + ", " +
                    docTypeIDField + " = " + ((rule.DocTypeID != 0) ? rule.DocTypeID.ToString() : "null") + ", " +
                    personIDField + " = " + ((rule.PersonID != 0) ? rule.PersonID.ToString() : "null") + ", " +
                    docIDField + " = " + ((rule.DocID != 0) ? rule.DocID.ToString() : "null") + ", " +
                    nameField + " = '" + rule.Name + "' " +

                    " WHERE " + idField + " = @ID";
            else
                queryString =
                    "INSERT INTO " + tableName + " (" +
                    folderIDField + ", " +
                    showModeField + ", " +
                    senderIDField + ", " +
                    addresseeIDField + ", " +
                    signerIDField + ", " +
                    docTypeIDField + ", " +
                    personIDField + ", " +
                    docIDField + ", " +
                    nameField +
                    ") VALUES  (@FolderID, " +
                    ((rule.Mode != 0) ? rule.Mode.ToString() : "null") + ", " +
                    ((rule.SenderID != 0) ? rule.SenderID.ToString() : "null") + ", " +
                    ((rule.AddresseeID != 0) ? rule.AddresseeID.ToString() : "null") + ", " +
                    ((rule.SignerID != 0) ? rule.SignerID.ToString() : "null") + ", " +
                    ((rule.DocTypeID != 0) ? rule.DocTypeID.ToString() : "null") + ", " +
                    ((rule.PersonID != 0) ? rule.PersonID.ToString() : "null") + ", " +
                    ((rule.DocID != 0) ? rule.DocID.ToString() : "null") + ", '" + rule.Name + "')" +

					"; SELECT SCOPE_IDENTITY() AS " + identityField;

            return ExecID(queryString,
                          delegate(SqlCommand cmd)
                              {
                                  AddParam(cmd, "@Mode", SqlDbType.Int, rule.Mode);
                                  AddParam(cmd, "@SenderID", SqlDbType.Int, rule.SenderID);
                                  AddParam(cmd, "@AddresseeID", SqlDbType.Int, rule.AddresseeID);
                                  AddParam(cmd, "@SignerID", SqlDbType.Int, rule.SignerID);
                                  AddParam(cmd, "@DocTypeID", SqlDbType.Int, rule.DocTypeID);
                                  AddParam(cmd, "@PersonID", SqlDbType.Int, rule.PersonID);
                                  AddParam(cmd, "@DocID", SqlDbType.Int, rule.DocID);
                                  AddParam(cmd, "@Descr", SqlDbType.NVarChar, rule.Name);

                                  if (rule.ID > 0)
                                      AddParam(cmd, "@ID", SqlDbType.Int, rule.ID);
                                  else
                                      AddParam(cmd, "@FolderID", SqlDbType.Int, rule.FolderID);
                              }, rule.ID);
        }

        public bool RuleExists(Rule rule)
        {
            return GetIntField("SELECT TOP 1 " + idField +
                               " FROM " + tableName +
                               " WITH (NOLOCK) WHERE " +
                               folderIDField + " = @FolderID AND " +
                               showModeField + " = @Mode AND " +
                               senderIDField + " = @SenderID AND " +
                               addresseeIDField + " = @AddresseeID AND " +
                               signerIDField + " = @SignerID AND " +
                               docTypeIDField + " = @DocTypeID AND " +
                               personIDField + " = @PersonID AND " +
                               docIDField + " = @DocID", idField,
                               delegate(SqlCommand cmd)
                                   {
                                       AddParam(cmd, "@FolderID", SqlDbType.Int, rule.FolderID);
                                       AddParam(cmd, "@Mode", SqlDbType.Int, rule.Mode);
                                       AddParam(cmd, "@SenderID", SqlDbType.Int, rule.SenderID);
                                       AddParam(cmd, "@AddresseeID", SqlDbType.Int, rule.AddresseeID);
                                       AddParam(cmd, "@SignerID", SqlDbType.Int, rule.SignerID);
                                       AddParam(cmd, "@DocTypeID", SqlDbType.Int, rule.DocTypeID);
                                       AddParam(cmd, "@PersonID", SqlDbType.Int, rule.PersonID);
                                       AddParam(cmd, "@DocID", SqlDbType.Int, rule.DocID);
                                   }) > 0;
        }

        public bool RuleNameExists(int folderID, string name)
        {
            return FieldExists("WHERE " +
                               folderIDField + " = @FolderID AND " +
                               nameField + " = @Descr",
                               delegate(SqlCommand cmd)
                                   {
                                       AddParam(cmd, "@FolderID", SqlDbType.Int, folderID);
                                       AddParam(cmd, "@Descr", SqlDbType.NVarChar, name);
                                   });
        }

        public void ApplyFolderRule(int КодПравилаПапкиДокументов, int КодПапкиПрименения, bool ПрименятьВоВложенных)
        {
            GetCount("sp_ПрименитьПравилоПапкиДокументов",
                     delegate(SqlCommand cmd)
                         {
                             cmd.CommandType = CommandType.StoredProcedure;

                             AddParam(cmd, "@КодПравилаПапкиДокументов", SqlDbType.Int, КодПравилаПапкиДокументов);
                             if (КодПапкиПрименения > 0)
                                 AddParam(cmd, "@КодПапкиПрименения", SqlDbType.Int, КодПапкиПрименения);

                             AddParam(cmd, "@ПрименятьВоВложенных", SqlDbType.Bit, ПрименятьВоВложенных);
                         });
        }
    }
}