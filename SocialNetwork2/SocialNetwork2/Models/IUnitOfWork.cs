//-
using System;
using System.Threading;
using System.Threading.Tasks;


namespace SocialNetwork2.Models;

public interface IUnitOfWork : IDisposable
{
    int SaveChanges(bool ensureAutoHistory = false);

    IRepository<TEntity> GetRepository<TEntity>(bool hasCustomRepository = true) where TEntity : class;
}
