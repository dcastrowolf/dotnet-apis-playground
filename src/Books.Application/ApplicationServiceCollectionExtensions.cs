using Books.Application.Database;
using Books.Application.Repositories;
using Books.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Books.Application
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddSingleton<IBooksRepository, BooksRepository>();
            services.AddSingleton<IBooksService, BooksService>();
            services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Singleton);
            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
            services.AddSingleton<DbInitializer>();
            return services;
        }
    }

}
