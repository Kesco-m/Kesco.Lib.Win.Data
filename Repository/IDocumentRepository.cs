namespace Kesco.Lib.Win.Data.Repository
{
    /// <summary>
    /// Интерфейс поставщика данных Документы
    /// </summary>
    public interface IDocumentRepository
    {
        /// <summary>
        /// Удалить документ.
        /// </summary>
        /// <param name="mainDocId"></param>
        /// <param name="toDeleteDocId">Удаляемый документ, для Merge</param>
        /// <param name="delete"></param>
        /// <returns></returns>
        bool DeleteDoc(int mainDocId, int toDeleteDocId, bool delete);
    }
}
