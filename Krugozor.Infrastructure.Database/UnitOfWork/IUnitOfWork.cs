using System;
using System.Threading.Tasks;
using Core.Models;

namespace Krugozor.Infrastructure.Database.UnitOfWork
{
  public interface IUnitOfWork : IDisposable
  {
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task<int> Complete();
  }
}