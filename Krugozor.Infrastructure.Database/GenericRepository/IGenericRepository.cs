using System.Collections.Generic;
using System.Threading.Tasks;
using Krugozor.Infrastructure.Specifications;
using Core.Models;

namespace Krugozor.Infrastructure.Database

{
  public interface IGenericRepository<T> where T : BaseEntity
  {
    Task<T> GetByIdAsync(int id);
    // Task<IReadOnlyList<T>> ListAllAsync();
    Task<T> GetEntityWithSpec(ISpecification<T> spec);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    Task<int> CountAsync(ISpecification<T> spec);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    void Delete(T entity);


  }
}