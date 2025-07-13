//-
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace SocialNetwork2.Models;

public interface IRepository<T> where T : class
{
    IEnumerable<T> GetAll();
    Task<IEnumerable<T>> GetAllAsync();
    T? Get(int id);
    Task<T?> GetAsync(int id);
    void Create(T item);
    Task CreateAsync(T item);
    void Update(T item);
    Task UpdateAsync(T item);
    void Delete(T item);
    Task DeleteAsync(T item);
}
