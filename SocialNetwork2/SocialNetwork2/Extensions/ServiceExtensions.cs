//-
using System;
using Microsoft.Extensions.DependencyInjection;

using SocialNetwork2.Models;


namespace SocialNetwork2.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        // services.AddSingleton<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static IServiceCollection AddCustomRepository<TEntity, IRepository>(this IServiceCollection services)
                where TEntity : class
                where IRepository : class, IRepository<TEntity>
    {
        // services.AddSingleton<IRepository<TEntity>, IRepository>();
        services.AddScoped<IRepository<TEntity>, IRepository>();

        return services;
    }
}
