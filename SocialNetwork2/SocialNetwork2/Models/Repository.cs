//-
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SocialNetwork2.Models;

public class Repository<T> : IRepository<T> where T : class
{
    protected DbContext _db;

    public DbSet<T> Set { get; private set; }


    public Repository(ApplicationDbContext db)
    {
        _db = db;

        var set = _db.Set<T>();
        set.Load();
        Set = set;
    }

    public void Create(T item)
    {
        Set.Add(item);
        _db.SaveChanges();
    }

    public async Task CreateAsync(T item)
    {
        await Set.AddAsync(item);
        await _db.SaveChangesAsync();
    }

    public void Delete(T item)
    {
        Set.Remove(item);
        _db.SaveChanges();
    }

    public async Task DeleteAsync(T item)
    {
        Set.Remove(item);
        await _db.SaveChangesAsync();
    }

    public T? Get(int id)
    {
        return Set.Find(id);
    }

    public async Task<T?> GetAsync(int id)
    {
        return await Set.FindAsync(id);
    }

    public IEnumerable<T> GetAll()
    {
        return Set;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Set.AsQueryable().ToListAsync();
    }

    public void Update(T item)
    {
        Set.Update(item);
        _db.SaveChanges();
    }

    public async Task UpdateAsync(T item)
    {
        Set.Update(item);
        await _db.SaveChangesAsync();
    }
}
