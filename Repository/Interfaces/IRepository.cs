using System.Linq.Expressions;

namespace Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        // Basic CRUD
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteByIdAsync(int id);
        Task DeleteByIdAsync(string id);
        
        // Query methods
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        
        // Paging
        Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);
        
        // Save changes
        Task<int> SaveChangesAsync();
    }
}

