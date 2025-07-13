//-
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using AutoMapper;

using SocialNetwork2.Tools;
using SocialNetwork2.Models;
using SocialNetwork2.Extensions;
using SocialNetwork2.SignalR;


namespace SocialNetwork2;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Подключаем автомаппинг
        var mapperConfig = new MapperConfiguration(v =>
        {
            v.AddProfile(new MappingProfile());
        });

        IMapper mapper = mapperConfig.CreateMapper();
        builder.Services.AddSingleton(mapper);

        // получаем строку подключения из файла конфигурации
        string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
        // обновляем публичные значения реальными значениями из приватной области
        connection = ConnectionTools.GetConnectionString(connection);

        // добавляем контекст ApplicationContext в качестве сервиса в приложение
        //  lifetime is in according with methods of the ServiceExtensions class
        builder.Services
            .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection),
                ServiceLifetime.Scoped);

        builder.Services
            .AddUnitOfWork()
                .AddCustomRepository<Friend, FriendsRepository>()
                .AddCustomRepository<Message, MessageRepository>();

        // модель работы с пользователями и работа с EF
        //  дефолтные значения: максимальная степень защиты
        //  - требуются буквы, цифры, заглавные буквы, длина пароля от 12 символов
        //  задаем менее жесткие ограничения
        // IdentityRole - базовая роль Identity
        builder.Services
            .AddIdentity<User, IdentityRole>(opts =>
            {
                opts.Password.RequiredLength = 5;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>();

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // подключема сервисы SignalR
        builder.Services.AddSignalR();

        // создаем в учебных целях экземпляр .Net клиента для SignalR
        // задача обновления страницы по сообщению работает на JS без участия .Net клиента
        //  .Net клиент отправляет сообщения в консоль в учебных целях
        ChatClient chatClient = new("http://localhost:5000" + "/chatHub");
        builder.Services.AddSingleton(chatClient);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        // SignalR broadcasting
        // ChatHub будет обрабатывать запросы по пути /chatHub
        app.MapHub<ChatHub>("/chatHub", options =>
        {
            options.Transports =
                HttpTransportType.WebSockets
                | HttpTransportType.LongPolling;
        });

        app.MapStaticAssets();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();

        // закрываем экземпляр .Net клиента для SignalR
        await chatClient.DisposeAsync();
    }
}
