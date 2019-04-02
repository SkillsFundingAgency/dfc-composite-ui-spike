using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ncs.Prototype.Interfaces
{
    public interface IStorage
    {
        Task Add<T>(string databaseId, string collectionId, T document);
        Task<T> Get<T>(string databaseId, string collectionId, string documentId);
        Task<List<T>> Search<T>(string databaseId, string collectionId, Expression<Func<T, bool>> expression);
        Task Update<T>(string databaseId, string collectionId, string documentId, T document);
        Task Delete(string databaseId, string collectionId, string documentId);
    }
}
