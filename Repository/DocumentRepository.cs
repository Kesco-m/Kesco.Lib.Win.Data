using System.Collections.Generic;
using Kesco.Lib.Win.Data.DALC.Documents;

namespace Kesco.Lib.Win.Data.Repository
{
    /// <summary>
    /// Поставщик данных Документы
    /// </summary>
    public sealed class DocumentRepository : IDocumentRepository
    {
        static DocumentRepository()
        {
            DeletedDocs = new List<int>(50);
        }

        /// <summary>
        /// Список удаленных документов
        /// </summary>
        private static readonly List<int> DeletedDocs;
        private static string _connectionStringDocument; // строка подключения к БД
        private static DocumentDALC _docDalc;

        /// <summary>
        /// Инициализация поставщика
        /// </summary>
        public static void Init(string connectionStringDocument)
        {
            _connectionStringDocument = connectionStringDocument;
            _docDalc = new DocumentDALC(_connectionStringDocument);
        }

        /// <summary>
        ///  Удалить документ
        /// </summary>
        /// <param name="mainDocId">Документ к обеденению с удаляемым</param>
        /// <param name="toDeleteDocId"></param>
        /// <param name="delete">Выполнить</param>
        /// <returns></returns>
        public bool DeleteDoc(int mainDocId, int toDeleteDocId, bool delete)
        {
            // Если не удаляем прямой вызов DALC
            if (!delete)
                return _docDalc.DeleteDoc(-1, toDeleteDocId, false);

            // Проверка, если уже документ удален
            if (DeletedDocs.Contains(toDeleteDocId))
                return true;

            // Удаление
            bool deleted = _docDalc.DeleteDoc(-1, toDeleteDocId, true);

            // Сохраняем в списке удаленных, если успешно удалили
            if (deleted && !DeletedDocs.Contains(toDeleteDocId))
            {
                if(DeletedDocs.Count > 50)
                    DeletedDocs.RemoveAt(0);

                DeletedDocs.Add(toDeleteDocId);
            }

            return deleted;
        }
    }
}
