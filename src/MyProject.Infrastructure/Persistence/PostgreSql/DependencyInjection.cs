using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyProject.Application.Common.Interfaces;
using MyProject.Application.Users.Interfaces;
using MyProject.Domain.Repositories;
using MyProject.Infrastructure.Persistence.PostgreSql.Queries;
using MyProject.Infrastructure.Persistence.PostgreSql.Repositories;
using MyProject.Infrastructure.Persistence.PostgreSql.Transactions;

namespace MyProject.Infrastructure.Persistence.PostgreSql;

internal static class PostgreSqlDependencyInjection
{
    public static IServiceCollection AddPostgreSql(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (!configuration.GetValue("PostgreSql:Enabled", true))
        {
            return services;
        }

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddHealthChecks().AddDbContextCheck<AppDbContext>("postgresql");
        services.AddScoped<IUnitOfWork>(serviceProvider =>
            serviceProvider.GetRequiredService<AppDbContext>());
        services.AddScoped<ITransactionManager, EfTransactionManager>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserQueries, UserQueries>();

        return services;
    }
}
