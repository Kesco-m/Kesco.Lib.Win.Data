using System.Data;
using System.Data.SqlClient;
using Kesco.Lib.Win.Data.DALC.Directory;
using Kesco.Lib.Win.Data.Documents;
using System.Threading;

namespace Kesco.Lib.Win.Data.DALC.Documents
{
    /// <summary>
    /// Summary description for TransactionDALC.
    /// </summary>
    public class TransactionDALC : DALC
    {
        private const string resTypeIDField = "КодТипаРесурса";
        private const string dateField = "Дата";

        private const string docMainIDField = "КодДокументаОснования";
        private const string docSuccessIDField = "КодДокументаПодтверждения";

        private const string personBeforeIDField = "КодЛицаДО";
        private const string personAfterIDField = "КодЛицаПОСЛЕ";

        private const string storyBeforeIDField = "КодСкладаДО";
        private const string storyAfterIDField = "КодСкладаПОСЛЕ";

        private const string sumRField = "СуммаРуб";
        private const string moneyIDField = "КодВалюты";
        private const string sumFeild = "Сумма";
        private const string resIDField = "КодРесурса";
        private const string countesField = "Количество";
        private const string transactionTypeIDField = "КодТипаТранзакции";

        private const string spDocTarnsaction = "sp_ТранзакцииДокумента";

        private string transactionTypeTableName;
        private string transactionTypeField;

        private string personTableName;
        private string personIDField;
        private string personNameField;

        private string resourceTableName;
        private string resourceNameField;
        private string resourceParentField;
        private string resourcePrecisionField;
        private string resourceUnitDimensionIDField;

        private string storeTableName;
        private string storeNameField;
        private string storeIDField;
        private string storeTypeIDFeild;
        private string storeKeeperIDField;
        private string storeKeeperField;

        private string unitDimensionTableName;
        private string unitDimensionNameField;

        private const string afterPostfix = "ПОСЛЕ";
        private const string beforePostfix = "ДО";

        private PersonDALC personData;
        private TransactionTypeDALC transactionTypeData;
        private ResourceDALC resourceData;
        private StoryDALC storeData;
        private UnitDimensionDALC unitDimensionData;

        public TransactionDALC(string connectionString)
            : base(connectionString)
        {
            tableName = "Документы.dbo.vwТранзакции";

            idField = "КодТранзакции";
            nameField = "Транзакция";

            transactionTypeData = new TransactionTypeDALC(null);
            transactionTypeTableName = transactionTypeData.TableName;
            transactionTypeField = transactionTypeData.NameField;

            personData = new PersonDALC(null);

            personTableName = personData.TableName;
            personIDField = personData.IDField;
            personNameField = personData.NameField;

            resourceData = new ResourceDALC(null);
            resourceTableName = resourceData.TableName;
            resourceNameField = resourceData.UnitRusField;
            resourceParentField = resourceData.ParentField;
            resourcePrecisionField = resourceData.PrecisionField;
            resourceUnitDimensionIDField = resourceData.UnitDimensionIDField;

            storeData = new StoryDALC(null);
            storeTableName = storeData.TableName;
            storeIDField = storeData.IDField;
            storeNameField = storeData.NameField;
            storeKeeperIDField = storeData.KeeperIDField;
            storeKeeperField = storeData.KeeperField;
            storeTypeIDFeild = storeData.StoreTypeIDField;

            unitDimensionData = new UnitDimensionDALC(null);
            unitDimensionTableName = unitDimensionData.TableName;
            unitDimensionNameField = unitDimensionData.NameField;
        }

        #region Accessors

        public string ResursTypeIDField
        {
            get { return resTypeIDField; }
        }

        public string DateField
        {
            get { return dateField; }
        }

        public string DocMainIDField
        {
            get { return docMainIDField; }
        }

        public string DocSuccessIDField
        {
            get { return docSuccessIDField; }
        }

        public string PersonBeforeIDField
        {
            get { return personBeforeIDField; }
        }

        public string PersonAfterIDField
        {
            get { return personAfterIDField; }
        }

        public string StoryBeforeIDField
        {
            get { return storyBeforeIDField; }
        }

        public string StoryAfterIDField
        {
            get { return storyAfterIDField; }
        }

        public string SumRField
        {
            get { return sumRField; }
        }

        public string MoneyIDField
        {
            get { return moneyIDField; }
        }

        public string ResIDField
        {
            get { return resIDField; }
        }

        public string SumFeild
        {
            get { return sumFeild; }
        }

        public string CountesField
        {
            get { return countesField; }
        }

        public string PersonIDField
        {
            get { return personIDField; }
        }

        public string PersonNameField
        {
            get { return personNameField; }
        }

        public string PersonTableName
        {
            get { return personTableName; }
        }

        public string TransactionTypeField
        {
            get { return transactionTypeField; }
        }

        public string TransactionTypeTableName
        {
            get { return transactionTypeTableName; }
        }

        public string TransactionTypeIDField
        {
            get { return transactionTypeIDField; }
        }

        public string PersonAfterField
        {
            get { return personNameField + afterPostfix; }
        }

        public string PersonBeforeField
        {
            get { return personNameField + beforePostfix; }
        }

        public string ResourceNameField
        {
            get { return resourceNameField; }
        }

        public string ResourcePrecisionField
        {
            get { return resourcePrecisionField; }
        }

        public string ResourceUnitDimensionIDField
        {
            get { return resourceUnitDimensionIDField; }
        }

        public string ResourceParentField
        {
            get { return resourceParentField; }
        }

        public string StoreIDField
        {
            get { return storeIDField; }
        }

        public string StoreNameField
        {
            get { return storeNameField; }
        }

        public string StoreNameAfterField
        {
            get { return storeNameField + afterPostfix; }
        }

        public string StoreNameBeforeField
        {
            get { return storeNameField + beforePostfix; }
        }

        public string StoreTypeIDFeild
        {
            get { return storeTypeIDFeild; }
        }

        public string StoreTypeIDAfterFeild
        {
            get { return storeTypeIDFeild + afterPostfix; }
        }

        public string StoreTypeIDBeforeFeild
        {
            get { return storeTypeIDFeild + beforePostfix; }
        }

        public string StoreKeeperIDField
        {
            get { return storeKeeperIDField; }
        }

        public string StoreKeeperIDAfterField
        {
            get { return storeKeeperIDField + afterPostfix; }
        }

        public string StoreKeeperIDBeforeField
        {
            get { return storeKeeperIDField + beforePostfix; }
        }

        public string StoreKeeperField
        {
            get { return storeKeeperField; }
        }

        public string StoreKeeperAfterField
        {
            get { return storeKeeperField + afterPostfix; }
        }

        public string StoreKeeperBeforeField
        {
            get { return storeKeeperField + beforePostfix; }
        }

        public string StoreTableName
        {
            get { return storeTableName; }
        }

        public string UnitDimensionNameField
        {
            get { return unitDimensionNameField; }
        }

        #endregion

        #region GetData

        public int DocHasTransactions(int docID)
        {
            return GetIdentityField(
                "SELECT TOP 1 " + idField +
                " FROM " + tableName +
                " WITH (NOLOCK) WHERE " + docMainIDField + " = @DocID" +
                " or " + docSuccessIDField + " = @DocID",
                delegate(SqlCommand cmd)
                    {
                        AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                    });
        }

        public int GetCount(int docID)
        {
            return GetCount("SELECT COUNT(*) " + countField +
                            " FROM " + tableName +
                            " WITH (NOLOCK) WHERE " + docMainIDField + " = @DocID" +
                            " or " + docSuccessIDField + " = @DocID",
                            delegate(SqlCommand cmd)
                                {
                                    AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                                });

        }

        public DataTable GetData(int docID, CancellationToken token)
        {
            return GetDataTable(spDocTarnsaction,
                                delegate(SqlCommand cmd)
                                    {
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        AddParam(cmd, "@КодДокумента", SqlDbType.Int, docID);
                                    }, token);
        }

        #endregion

        #region Change data

        public bool DeleteTransaction(int docID)
        {
            return Exec("DELETE " + tableName +
                        " WHERE " + docSuccessIDField + " = @DocID",
                        delegate(SqlCommand cmd)
                            {
                                AddParam(cmd, "@DocID", SqlDbType.Int, docID);
                            });
        }

        #endregion
    }
}