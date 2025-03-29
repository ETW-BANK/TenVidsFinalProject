using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TenVids.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {

      Task <IEnumerable<T>> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null,
            Func<IQueryable<T>,IOrderedQueryable<T>> orderby = null);
     Task< T> GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
        void Add(T entity);
        void Update(T entity,T destination);  
        void Remove(T item);
        Task <bool> AnyAsync(Expression<Func<T, bool>> filter);

        Task<T>GetByIdAsync(int? id, string? includeProperties = null);
        void RemoveRange(IEnumerable<T> items);

        Task<int> Count(Expression<Func<T, bool>> filter = null);


    }
}
